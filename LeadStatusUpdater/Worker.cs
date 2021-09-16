using Exchange;
using LeadStatusUpdater.Common;
using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Services;
using MailExchange;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LeadStatusUpdater
{
    public class Worker : BackgroundService
    {
        private readonly ISetVipService _service;
        private readonly EmailPublisher _emailPublisher;


        private int _hourTimeSpan = 3600000;
        public Worker(ISetVipService service,
            EmailPublisher emailPublisher)
        {
            _emailPublisher = emailPublisher;
            _service = service;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            //sleep until 3.30
            Log.Information($"Worker started at: {DateTime.Now}");
            await base.StartAsync(cancellationToken);
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {

            Log.Information($"Worker stopped at: {DateTime.Now}");
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (ConverterService.RatesModel == null)
                {
                    await Task.Delay(2000, stoppingToken);
                    continue;
                }

                //var attempt = 0;

                Log.Information($"Cycle started at: {DateTime.Now}");
                //if (ConverterService.RatesModel == null && attempt < 1)
                //{
                //    Log.Warning($"{LogMessages.RatesNotProvided} first time");
                //    attempt++;
                //    Thread.Sleep(2000);
                //    continue;
                //}
                //if (ConverterService.RatesModel == null && attempt > 0)
                //{
                //    attempt = 0;
                //    Log.Warning($"{LogMessages.RatesNotProvided} twice, finished cycle");
                //    await Task.Delay(2000, stoppingToken);//timer
                //}
                //else
                //{
                try
                {
                    _service.Process();
                    Log.Information($"Cycle finished successfully at: {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                    var msg = new EmailModel { Subject = "AAa", Body = "QQQQ"};
                        
                    await _emailPublisher.PublishEmail(msg);
                }
                finally
                {
                    await Task.Delay(2000, stoppingToken);//timer
                }
                //}
            }
        }
    }
}
