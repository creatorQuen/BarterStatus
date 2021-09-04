using Newtonsoft.Json;
using RestSharp;
using SwapLogCor.Models;
using SwapLogCor.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapLogCor
{
    public class RequestsSender
    {
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;

        public List<LeadShortModel> GetAllLeadsId()
        {
            var request = _requestHelper.CreateGetRequest("");
            var response = _client.Execute<string>(request);

            var result = JsonConvert.DeserializeObject(response.Data);

        

            return new List<LeadShortModel>();
        }

        public List<TransactionBusinessModel> GetTransactionsWithoutWithdrawsByLead(LeadShortModel lead)
        {
            return new List<TransactionBusinessModel>();
        }

        public void DeleteWithdrawTransactionsFromAllTransactions(List<TransactionBusinessModel> transactions)
        {

        }


    }
}
