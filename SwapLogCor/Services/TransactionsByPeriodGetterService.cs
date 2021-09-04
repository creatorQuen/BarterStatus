using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapLogCor.Services
{
    public class TransactionsByPeriodGetterService
    {
        private string _url;
        private RestClient _client;
        private RestRequest _request;
        private IRestResponse<string> _response;

        public TransactionsByPeriodGetterService()
        {
            _client = new RestClient("localhost");
            _request = new RestRequest(Method.GET);
        }

        public void GetTransactionsByPeriod(int id)
        {
            var getTransactions = new RestRequest(string.Format(Constants.CRM_TRANSACTIONS_BY_PERIOD, id), Method.GET);
            var response = _client.Execute<string>(getTransactions);
            var transactions = response.Data;

        }
    }
}
