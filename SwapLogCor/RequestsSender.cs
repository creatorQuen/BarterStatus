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
    public class RequestsSender : IRequestsSender
    {
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;

        public RequestsSender()
        {

        }

        public List<LeadShortModel> GetAllLeads()
        {
            var request = _requestHelper.CreateGetRequest("");
            var response = _client.Execute<string>(request);

            var result = JsonConvert.DeserializeObject<List<LeadShortModel>>(response.Data);

            return result;
        }

        public List<TransactionBusinessModel> GetTransactionsWithoutWithdrawsByLead(LeadShortModel lead)
        {

            return new List<TransactionBusinessModel>();
        }

        public void DeleteWithdrawTransactionsFromAllTransactions(List<TransactionBusinessModel> transactions)
        {

        }

        public List<TransactionBusinessModel> GetTransactionsByPeriod(int leadId, int period)
        {
            var getTransactions = new RestRequest(string.Format(Constants.CRM_TRANSACTIONS_BY_PERIOD, leadId), Method.GET);
            var response = _client.Execute<string>(getTransactions);
            var transactions = response.Data;
            return new List<TransactionBusinessModel>();
        }
    }
}
