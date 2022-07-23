using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string outputTemplate = "{Timestamp:yyyy-MM-dd HH: mm: ss.fff} [{Level}] {Message} { NewLine} { Exception} TEST";
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .WriteTo.File("C:\\Logs\\Demo.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: outputTemplate).CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog();
    }
}