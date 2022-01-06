using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTO;
using WebApiAutores.Service;

namespace WebApiAutores.Controllers.v1
{
    [Route("api/v1/cuenta")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentaController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            this.dataProtector = dataProtectionProvider.CreateProtector("valor_unico_secreto");
        }
        [HttpGet("hash/{texto}")]
        public ActionResult RealizarHash(string texto)
        {
            var result1 = hashService.Hash(texto);
            var result2 = hashService.Hash(texto);
            return Ok(new
            {
                result1,
                result2
            });
        }
        [HttpGet("encryptar")]
        public ActionResult Encriptar()
        {

            var textoPlano = "Andy jesu macias gomez";
            var textoCifrado = dataProtector.Protect(textoPlano);
            var textoDesencriptado = dataProtector.Unprotect(textoCifrado);
            return Ok(new
            {
                textoPlano,
                textoCifrado,
                textoDesencriptado

            });
        }
        [HttpGet("encryptarTiempo")]
        public ActionResult EncriptarTiempo()
        {
            var protector = dataProtector.ToTimeLimitedDataProtector();
            var textoPlano = "Andy jesu macias gomez";
            var textoCifrado = protector.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5));
            var textoDesencriptado = protector.Unprotect(textoCifrado);
            return Ok(new
            {
                textoPlano,
                textoCifrado,
                textoDesencriptado

            });
        }
        [HttpPost("registrar")]
        public async Task<ActionResult<RespAuthenticate>> Registro([FromBody] Credenciales credenciales)
        {
            var usuario = new IdentityUser
            {
                UserName = credenciales.Email,
                Email = credenciales.Email
            };
            var result = await userManager.CreateAsync(user: usuario, password: credenciales.Password);

            if (result.Succeeded)
            {
                return await CreateToken(credenciales);
            }
            else
            {
                return BadRequest(result.Errors);
            }

        }
        [HttpPost("login")]
        public async Task<ActionResult<RespAuthenticate>> Login([FromBody] Credenciales credenciales)
        {
            var resultado = await signInManager.PasswordSignInAsync(credenciales.Email, credenciales.Password, isPersistent: false, lockoutOnFailure: false);
            if (resultado.Succeeded) return await CreateToken(credenciales);

            return BadRequest("Bad Credentials");
        }
        [HttpGet("renovar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespAuthenticate>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim?.Value ?? "";
            var credentials = new Credenciales
            {
                Email = email,
            };
            return await CreateToken(credentials);


        }


        private async Task<RespAuthenticate> CreateToken(Credenciales credenciales)
        {
            var claims = new List<Claim>() {
                new Claim("email", credenciales.Email),
                new Claim("example", "this examples test")
            };
            var usuario = await userManager.FindByEmailAsync(email: credenciales.Email);
            var claimsDb = await userManager.GetClaimsAsync(user: usuario);
            claims.AddRange(claimsDb);
            var jwtKey = configuration["jwtKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(20);
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: credentials);
            return new RespAuthenticate()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiration
            };
        }

        [HttpPost("admin")]
        public async Task<ActionResult> ToAdmin([FromBody] EditarAdmin editarAdmin)
        {
            var user = await userManager.FindByEmailAsync(email: editarAdmin.Email);
            await userManager.RemoveClaimAsync(user, new Claim("Admin", "admin default"));
            await userManager.AddClaimAsync(user, new Claim("Admin", "admin default"));
            return NoContent();
        }
        [HttpPost("removeAdmin")]
        public async Task<ActionResult> RemoveAdmin([FromBody] EditarAdmin editarAdmin)
        {
            var user = await userManager.FindByEmailAsync(email: editarAdmin.Email);
            await userManager.RemoveClaimAsync(user, new Claim("Admin", "admin default"));
            return NoContent();
        }


    }
}
