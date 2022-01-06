using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutores.DTO;

namespace WebApiAutores.Filter
{
    public class HATEOASAutorFilterAttribute : HATEOASFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOS(context);
            if (!debeIncluir)
            {
                await next();
                return;
            }
            var resultado = context.Result as ObjectResult;
            var modelo = resultado.Value as AutorDto ?? throw new ArgumentException("Se esperar una instancia de autopr dto");
            await next();
        }
    }
}
