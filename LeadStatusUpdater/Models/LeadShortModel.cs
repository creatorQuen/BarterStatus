using System.Collections.Generic;

namespace LeadStatusUpdater.Models
{
    public class LeadShortModel
    {
        public int Id { get; set; }
        public string BirthDate { get; set; }
        public List<int> Accounts { get; set; }

    }
}
