using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using BarterStatus.Constants;
using BarterStatus.Models;
using BarterStatus.Requests;
using BarterStatus.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BarterStatus
{
    public class RequestsSender : IRequestsSender
    {
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;

        public RequestsSender(IOptions<AppSettings> options)
        {
            _client = new RestClient(options.Value.CRM_URL);
            _requestHelper = new RequestHelper();
        }

        public List<LeadShortModel> GetAllLeads()
        {
            var request = _requestHelper.CreateGetRequest(Endpoints.GET_ALL_LEADS);
            var response = _client.Execute<string>(request);

            var result = JsonConvert.DeserializeObject<List<LeadShortModel>>(response.Data);

            return result;
        }


        public List<TransactionBusinessModel> GetTransactionsByPeriod(LeadShortModel lead, PeriodModel period)
        {
            List<TransactionBusinessModel> transactionsList = new List<TransactionBusinessModel>();
            foreach(var acc in lead.Accounts)
            {
                period.AccountId = acc;
                var request = _requestHelper.CreatePostRequest(Endpoints.GET_TRANSACTIONS_BY_PERIOD, period);
                var response = _client.Execute<string>(request);
                var result = JsonConvert.DeserializeObject<List<TransactionBusinessModel>>(response.Data);
                transactionsList.Union(result);
            }
            return transactionsList;
        }

        public void SetVipStatus(int leadId, bool status)
        {

        }
    }
}
