using Serilog;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Configuration.FileExtensions;

namespace SwapLogCor
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = AppStartup();

            var dataService = ActivatorUtilities.CreateInstance<DataService>(host.Services);

            dataService.Connect();
        }

        static void ConfigSetup(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
        }

        static IHost AppStartup()
        {
            var builder = new ConfigurationBuilder();
            ConfigSetup(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();


            var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                services.AddTransient<IDataService, DataService>();
            })
            .UseSerilog()
            .Build();

            return host;
        }
    }
}
