using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapLogCor.Models
{
    public class LeadShortModel
    {
        public int Id { get; set; }
        public string BirthDate { get; set; }
        public List<int> Accounts { get; set; }

    }
}
