using LeadStatusUpdater.Extensions;
using LeadStatusUpdater.Requests;
using LeadStatusUpdater.Services;
using LeadStatusUpdater.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace LeadStatusUpdater
{
    public class Program
    {
        //private const string _section_key = "ConnectionCRM";
        public static void Main(string[] args)
        {
            var configuration = CreateConfiguratuion();
            configuration.SetEnvironmentVariableForConfiguration();

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
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddOptions<AppSettings>()
                .Bind(configuration.GetSection(nameof(AppSettings)));

                    services.AddHostedService<Worker>();
                    services.AddTransient<ISetVipService, SetVipService>();
                    services.AddTransient<IRequestsSender, RequestsSender>();

                    //services.AddMassTransit(x =>
                    //{
                    //    x.AddConsumer<MailTransactionConsumer>();
                    //    x.AddConsumer<MailAdminConsumer>();
                    //    x.SetKebabCaseEndpointNameFormatter();
                    //    x.UsingRabbitMq((context, cfg) =>
                    //    {
                    //        cfg.ReceiveEndpoint(_queueTransaction, e =>
                    //        {
                    //            e.ConfigureConsumer<MailTransactionConsumer>(context);
                    //        });
                    //        cfg.ReceiveEndpoint(_queueAdmin, e =>
                    //        {
                    //            e.ConfigureConsumer<MailAdminConsumer>(context);
                    //        });
                    //    });
                    //});

                    //services.AddMassTransitHostedService();
                });
    }
}
