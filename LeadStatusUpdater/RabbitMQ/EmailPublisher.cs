using LeadStatusUpdater.Models;
using MailExchange;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Common
{
    public class EmailPublisher
    {
        IBusControl _busControl;
        public EmailPublisher()
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(configure: cfg =>
            {
                // 5672 Основной порт RabbitMQ
                cfg.Host("80.78.240.16", "/", h =>
                {
                    h.Username("nafanya");
                    h.Password("qwe!23");
                });
            });
        }
        public async Task PublishEmail(EmailModel message)
        {
            //var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await _busControl.StartAsync();
            try
            {
                
                await _busControl.Publish<IMailExchangeModel>(new
                {
                    Subject = message.Subject,
                    Body = message.Body,
                    MailAddresses = message.MailAddresses
                });
            }
            finally
            {
                await _busControl.StopAsync();
            }
        }
    }
}
