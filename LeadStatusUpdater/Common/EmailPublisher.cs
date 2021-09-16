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
                cfg.Host("localhost", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        }
        public async Task PublishEmail(EmailModel message)
        {
            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await _busControl.StartAsync(source.Token);
            try
            {
                
                await _busControl.Publish<IMailExchangeModel>(new
                {
                    Subject = message.Subject,
                    Body = message.Body,
                    MailAddresses = "merymal2696@gmail.com"
                });
            }
            finally
            {
                await _busControl.StopAsync();
            }
        }
    }
}
