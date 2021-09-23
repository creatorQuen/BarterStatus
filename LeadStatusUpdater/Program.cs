using LeadStatusUpdater.Common;
using LeadStatusUpdater.Extensions;
using LeadStatusUpdater.Requests;
using LeadStatusUpdater.Services;
using LeadStatusUpdater.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace LeadStatusUpdater
{
    public class Program
    {
        private const string _queue = "rates-queue-test";
        private const string _sectionKey = "AppSettings";
        private static string _rabbitHost = "RabbitMqAddress";
        private static string _rabbitPassword = "RabbitMqPassword";
        private static string _rabbitUsername = "RabbitMqUsername";
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
                              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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
                    services.AddTransient<IConverterService, ConverterService>();
                    services.AddTransient<IRabbitMqPublisher, RabbitMqPublisher>();

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<RatesConsumer>();
                        x.SetKebabCaseEndpointNameFormatter();
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(configuration.GetValue<string>($"{_sectionKey}:{_rabbitHost}"), h =>
                            {
                                h.Username(configuration.GetValue<string>($"{_sectionKey}:{_rabbitUsername}"));
                                h.Password(configuration.GetValue<string>($"{_sectionKey}:{_rabbitPassword}"));
                            });
                            cfg.ReceiveEndpoint(_queue, e =>
                            {
                                e.ConfigureConsumer<RatesConsumer>(context);
                            });
                        });
                    });

                    services.AddMassTransitHostedService();
                });
    }
}
