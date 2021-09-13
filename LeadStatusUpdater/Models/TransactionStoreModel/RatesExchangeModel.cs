using System.Collections.Generic;

namespace LeadStatusUpdater.Models
{
    public class RatesExchangeModel
    {
        public string Updated { get; set; }
        public string BaseCurrency { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
