using LeadStatusUpdater.Models;
using MailExchange;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Common
{
    public class RabbitMqPublisher
    {
        IBusControl _busControl;
        public RabbitMqPublisher()
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

        public async Task Start() => await _busControl.StartAsync();
        public async Task Stop() => await _busControl.StopAsync();
        public async Task PublishMessage(EmailModel message)
        {
            //var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            try
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
            catch(Exception ex)
            {
                //
            }
        }
    }
}
