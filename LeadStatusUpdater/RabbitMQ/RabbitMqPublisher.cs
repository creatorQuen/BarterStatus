using LeadStatusUpdater.Models;
using LeadStatusUpdater.Settings;
using MailExchange;
using MassTransit;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Common
{
    public class RabbitMqPublisher
    {
        IBusControl _busControl;
        
        public RabbitMqPublisher(IOptions<AppSettings> settings)
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(configure: cfg =>
            {
                cfg.Host(settings.Value.RabbitMqAddress, h =>
                {
                    h.Username(settings.Value.RabbitMqUsername);
                    h.Password(settings.Value.RabbitMqPassword);
                });
            });
        }

        public async Task Start() => await _busControl.StartAsync();
        public async Task Stop() => await _busControl.StopAsync();
        public async Task PublishMessage(EmailModel message)
        {
            await _busControl.Publish<IMailExchangeModel>(new
            {
                Subject = message.Subject,
                Body = message.Body,
                MailAddresses = message.MailAddresses,
                DisplayName = message.DisplayName,
                IsBodyHtml = message.IsBodyHtml
            });
        }
    }
}
