using Exchange;
using LeadStatusUpdater.Common;
using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Services;
using LeadStatusUpdater.Settings;
using MailExchange;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LeadStatusUpdater
{
    public class Worker : BackgroundService
    {
        private readonly ISetVipService _service;
        private readonly RabbitMqPublisher _emailPublisher;
        private Timer _timer;
        private readonly int _millisecondsDelay;
        private readonly int _millisecondsWhenLaunch;

        public Worker(ISetVipService service,
            RabbitMqPublisher emailPublisher,
            IOptions<AppSettings> settings)
        {
            _emailPublisher = emailPublisher;
            _service = service;
            _millisecondsDelay = settings.Value.MillisecondsDelay;
            _millisecondsWhenLaunch = settings.Value.MillisecondsWhenLaunch;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
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
                await Task.Delay(2000);
                Log.Information($"Cycle started at: {DateTime.Now}");
                await _emailPublisher.Start();
                try
                {
                    Task.Run(() => _service.Process(new object())).Wait();
                    //await _service.Process(new object());
                    Log.Information($"Cycle finished successfully at: {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Log.Fatal($"{ex.Message}\nStackTrace: {ex.StackTrace}");
                    await _emailPublisher.PublishMessage(EmailMessage.GetBadEmail(ex.Message));
                }
                finally
                {
                    await _emailPublisher.Stop();
                    await Task.Delay(CountTimeToSleep(), stoppingToken);
                }
            }
        }

        public int CountTimeToSleep()
        {
            int sleepTime;
            var nowMiliS = (long)((DateTime.Now.TimeOfDay).TotalMilliseconds);

            sleepTime = _millisecondsWhenLaunch < nowMiliS ? 
                (int)(_millisecondsDelay - nowMiliS + _millisecondsWhenLaunch) :
                (int)(_millisecondsWhenLaunch - nowMiliS);

            return sleepTime;
        }

        //private void SetTimer()
        //{
        //    var act = new TimerCallback(_service.Process);
        //    _timer = new Timer(act, default, 0, _millisecondsWhenLaunch);
        //}
    }
}
