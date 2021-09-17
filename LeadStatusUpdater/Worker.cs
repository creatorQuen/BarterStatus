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
        private readonly EmailPublisher _emailPublisher;
        private Timer _timer;
        private readonly int _millisecondsDelay;
        private readonly int _millisecondsWhenLaunch;

        public Worker(ISetVipService service,
            EmailPublisher emailPublisher,
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
            Log.Information($"Djiga Djiga zapuskay {DateTime.Now}");
            while (!stoppingToken.IsCancellationRequested)
            {
                if (ConverterService.RatesModel == null)
                {
                    await Task.Delay(2000, stoppingToken);
                    continue;
                }
                _emailPublisher.Sart();
                Log.Information($"Cycle started at: {DateTime.Now}");
                //await _emailPublisher.PublishEmail(new EmailModel { Subject = "START", Body = $"Updater started at: {DateTime.Now}", DisplayName = "StatusUpdater", MailAddresses = "merymal2696@gmail.com" });//add to consts
                try
                {
                    HowTimeSleep();
                    SetTimer();
                    Log.Information($"Cycle finished successfully at: {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                    await _emailPublisher.PublishEmail(new EmailModel { Subject = "PIZDA", Body = $"VSE SLOMALOSYA: {ex.Message}", DisplayName = "StatusUpdater", MailAddresses = "merymal2696@gmail.com" });//add to consts
                }
                finally
                {
                    _emailPublisher.Stop();
                    var sleepTime = HowTimeSleep();
                    await Task.Delay(sleepTime, stoppingToken);//timer
                }
            }
        }

        public int HowTimeSleep()
        {
            int sleepTime;
            var nowMiliS = (long)((DateTime.Now.TimeOfDay).TotalMilliseconds);

            if (_millisecondsWhenLaunch < nowMiliS)
            {
                sleepTime = (int)(_millisecondsDelay - nowMiliS + _millisecondsWhenLaunch);
            }
            else
            {
                sleepTime = (int)(_millisecondsWhenLaunch - nowMiliS);
            }

            Thread.Sleep(sleepTime);
            return sleepTime;
        }

        private void SetTimer()
        {
            var act = new TimerCallback(_service.Process);
            _timer = new Timer(act, default, 0, _millisecondsWhenLaunch);
        }
    }
}
