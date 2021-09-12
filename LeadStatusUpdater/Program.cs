using LeadStatusUpdater.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace LeadStatusUpdater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = CreateConfiguratuion();

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
                    services.AddHostedService<Worker>();
                });

        //, IConfiguration configuration
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                 {
                     //services.AddOptions<EmailConfig>().Bind(configuration.GetSection(_sectionKey));
                     services.AddTransient<ISetVipService, SetVipService>();
                     services.AddHostedService<Worker>();

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
