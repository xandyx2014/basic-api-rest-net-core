using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filter
{
    public class HATEOASFilterAttribute : ResultFilterAttribute
    {
        protected bool DebeIncluirHATEOS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;
            if (!esRespuestaExitosa(result))
            {
                return false;
            }
            var cabecera = context.HttpContext.Request.Headers["HATEOAS"];
            if (cabecera.Count == 0)
            {
                return false;
            }
            var valor = cabecera[0];
            if (!valor.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }
        private bool esRespuestaExitosa(ObjectResult objectResult)
        {
            if (objectResult == null || objectResult.Value == null)
            {
                return false;
            }
            if (objectResult.StatusCode.HasValue && !objectResult.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }
            return true;
        }
    }
}
