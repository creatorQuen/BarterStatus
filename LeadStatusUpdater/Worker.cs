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
            Log.Information($"Sleeping: {DateTime.Now}");
            Thread.Sleep(1000);
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
                if(ConverterService.RatesModel == null)
                {
                    Log.Warning($"{LogMessages.RatesNotProvided}");
                    Thread.Sleep(_hourTimeSpan);
                    continue;
                }
                _service.Process();

                await Task.Delay(_hourTimeSpan, stoppingToken);
            }
        }
    }
}
