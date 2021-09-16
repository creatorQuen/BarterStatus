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
            var attempt = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Information($"Cycle started at: {DateTime.Now}");
                if (ConverterService.RatesModel == null && attempt < 1)
                {
                    Log.Warning($"{LogMessages.RatesNotProvided} first time");
                    attempt++;
                    Thread.Sleep(2000);
                    continue;
                }
                if (ConverterService.RatesModel == null && attempt > 0)
                {
                    attempt = 0;
                    Log.Warning($"{LogMessages.RatesNotProvided} twice, finished cycle");
                    await Task.Delay(2000, stoppingToken);//timer
                }
                else
                {
                    try
                    {
                    var tets = ConverterService.RatesModel;
                        _service.Process();
                        Log.Information($"Cycle finished successfully at: {DateTime.Now}");
                    }
                    catch(Exception ex)
                    {
                        Log.Error(ex.Message);
                        //send email to admin
                    }
                    finally
                    {
                        await Task.Delay(2000, stoppingToken);//timer
                    }
                }
            }
        }
    }
}
