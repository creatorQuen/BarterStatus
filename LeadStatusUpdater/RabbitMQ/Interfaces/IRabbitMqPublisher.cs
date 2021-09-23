using LeadStatusUpdater.Models;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Common
{
    public interface IRabbitMqPublisher
    {
        Task Start();
        Task Stop();
        Task PublishMessage(EmailModel message);
    }
}
