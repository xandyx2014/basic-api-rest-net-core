using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTO;

namespace WebApiAutores.Controllers.v1
{
    [ApiController]
    [Route("api")]
    [HeaderAttributePresent("x-version", "1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOS>>> Get()
        {
            var admin = await authorizationService.AuthorizeAsync(User, "Admin");
            var datoHATEOS = new List<DatoHATEOS>();
            datoHATEOS.Add(new DatoHATEOS(
                enlance: Url.Link("ObtenerRoot", new { }),
                descripcion: "self",
                metodo: "GET"
                ));
            if (admin.Succeeded)
            {
                datoHATEOS.Add(new DatoHATEOS(
                enlance: Url.Link("ObtenerRoot", new { admin = "True" }),
                descripcion: "self",
                metodo: "GET"
                ));
            }
            return datoHATEOS;
        }
    }
}
