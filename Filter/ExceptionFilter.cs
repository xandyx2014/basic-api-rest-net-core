using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filter
{
    // Existen Diferente Tipos de filtros
    // Filtros de autorizacion
    // Filtros de recursos
    // Accion
    // Excepcion
    // Resultado

    // Maneras de aplciar filtro
    // Nivel de accion
    // nivel del controlador
    // nivel global
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilter> logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            this.logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
