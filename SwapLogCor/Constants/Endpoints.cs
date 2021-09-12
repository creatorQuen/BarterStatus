using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapLogCor.Constants
{
    public class Endpoints
    {
        public const string GetTransactionByPeriodEndpoint = "/api/Account/by-period";
        public const string GetAllLeadsEndpoint = "/api/Lead";
        public const string GetAllLeadsByFilterEndpoint = "/api/Lead/filter";
        public const string ChangeStatusEndpoint = "/api/Lead/{id}/role/{role}";

    }
}
