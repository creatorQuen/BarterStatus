using LeadStatusUpdater.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeadStatusUpdater.Services
{
    public interface ISetVipService
    {
        Task<bool> CheckOneLead(LeadOutputModel lead);
        void Process(object obj);
    }
}
