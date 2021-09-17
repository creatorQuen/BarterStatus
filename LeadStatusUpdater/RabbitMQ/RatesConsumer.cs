using Exchange;
using LeadStatusUpdater.Services;
using MassTransit;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Common
{
    public class RatesConsumer :
        IConsumer<RatesExchangeModel>
    {
        public async Task Consume(ConsumeContext<RatesExchangeModel> context)
        {
            ConverterService.RatesModel = context.Message;            
            await Task.CompletedTask;
        }
    }
}
