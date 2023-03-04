namespace Protobuf.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var loggerFactory = LoggerFactory
                .Create(loggingBuilder => loggingBuilder
                    .SetMinimumLevel(LogLevel.Information)
                    .AddConsole());

            var logger = loggerFactory.CreateLogger<Program>();
            try
            {
                logger.LogInformation("Starting host");
                CreateHostBuilder(args)
                    .Build()
                    .Run();
                logger.LogInformation("Host stopped");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Host terminated unexpectedly");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}