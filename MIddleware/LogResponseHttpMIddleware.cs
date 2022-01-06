namespace WebApiAutores.MIddleware
{
    // Clase de extension de metodos
    // para poder utilizar esta extension se tiene que llamar a using WebApiAutores.MIddleware
    public static class MIddlewareExtensions
    {
        // Metodo de extension 
        public static IApplicationBuilder UseLogResponseHttp(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LogResponseHttpMIddleware>();
        }
    }
    public class LogResponseHttpMIddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<LogResponseHttpMIddleware> logger;

        public LogResponseHttpMIddleware(RequestDelegate next, ILogger<LogResponseHttpMIddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }
        // debe tener un metodo publico llamado Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            using var ms = new MemoryStream();
            var original = context.Response.Body;
            context.Response.Body = ms;
            await next(context);
            ms.Seek(0, SeekOrigin.Begin);
            string response = new StreamReader(ms).ReadToEnd();
            ms.Seek(0, SeekOrigin.Begin);
            await ms.CopyToAsync(original);
            context.Response.Body = original;
            logger.LogInformation(message: response);
        }
    }
}
