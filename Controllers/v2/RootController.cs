using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTO;

namespace WebApiAutores.Controllers.v2
{
    [ApiController]
    [Route("api/")]
    [HeaderAttributePresent("x-version", "2")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRootv2")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var admin = await authorizationService.AuthorizeAsync(User, "Admin");
            var datoHATEOS = new List<DatoHATEOS>();
            datoHATEOS.Add(new DatoHATEOS(
                enlance: Url.Link("ObtenerRootv2", new { }),
                descripcion: "self",
                metodo: "GET"
                ));
            if (admin.Succeeded)
            {
                datoHATEOS.Add(new DatoHATEOS(
                enlance: Url.Link("ObtenerRootv2", new { admin = "True" }),
                descripcion: "self",
                metodo: "GET"
                ));
            }
            return Ok(new
            {
                data = datoHATEOS,
                version = 2
            });
        }
    }
}
