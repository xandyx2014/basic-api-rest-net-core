using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filter
{
    public class FilterAction : IActionFilter
    {
        private readonly ILogger<FilterAction> logger;

        public FilterAction(ILogger<FilterAction> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Antes de ejecutar");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Despues de ejecutar");
        }
    }
}
