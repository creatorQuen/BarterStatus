using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Services;
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

        private int _hourTimeSpan = 3600000;

        public Worker(ISetVipService service)
        {
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
            var attemt = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Information($"Cycle started at: {DateTime.Now}");
                if (ConverterService.RatesModel == null && attemt < 1)
                {
                    Log.Warning($"{LogMessages.RatesNotProvided} first time");
                    attemt++;
                    Thread.Sleep(_hourTimeSpan);
                    continue;
                }
                if(ConverterService.RatesModel == null && attemt > 0)
                {
                    Log.Warning($"{LogMessages.RatesNotProvided} twice, finished cycle");
                    await Task.Delay(_hourTimeSpan, stoppingToken);//timer
                }
                else
                {
                    _service.Process();
                    Log.Information($"Cycle finished successfully at: {DateTime.Now}");
                    await Task.Delay(_hourTimeSpan, stoppingToken);//timer
                }
            }
        }
    }
}
