using LeadStatusUpdater.Common;
using LeadStatusUpdater.Extensions;
using LeadStatusUpdater.Requests;
using LeadStatusUpdater.Services;
using LeadStatusUpdater.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace LeadStatusUpdater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = CreateConfiguratuion();
            configuration.SetEnvironmentVariableForConfiguration();
            configuration.ConfigureLogger();
            CreateHostBuilder(args, configuration).Build().Run();
        }

        public static IConfiguration CreateConfiguratuion()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                              .Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddOptions<AppSettings>()
                        .Bind(configuration.GetSection(nameof(AppSettings)));

                    services.AddHostedService<Worker>();
                    services.AddTransient<ISetVipService, SetVipService>();
                    services.AddTransient<IRequestsSender, RequestsSender>();
                    services.AddTransient<EmailPublisher>();

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<RatesConsumer>();
                        x.SetKebabCaseEndpointNameFormatter();
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            //cfg.Host("80.78.240.16", h =>
                            //{
                            //    h.Username("nafanya");
                            //    h.Password("qwe!23");
                            //});
                            cfg.ReceiveEndpoint("rates-queue-test", e =>
                            {
                                e.ConfigureConsumer<RatesConsumer>(context);
                            });
                        });
                    });

                    services.AddMassTransitHostedService();
                });
    }
}
