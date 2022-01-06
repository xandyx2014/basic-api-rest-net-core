namespace WebApiAutores.Service
{
    public class WritteFileHostedServices : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nameFile = "namefile1.txt";
        public WritteFileHostedServices(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Escribir("StartASync");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Escribir("StopAsync");
            return Task.CompletedTask;
        }
        protected void Escribir(string message)
        {
            var path = $@"{env.ContentRootPath}\wwwroot\{nameFile}";
            using StreamWriter writer = new StreamWriter(path, append: true);
            writer.WriteLine(message);
            
        }
    }
}
