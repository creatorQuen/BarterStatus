using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SwapLogCor.Constants;
using SwapLogCor.Enums;
using SwapLogCor.Models;
using SwapLogCor.Requests;
using SwapLogCor.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SwapLogCor
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
            var request = _requestHelper.CreateGetRequest(Endpoints.GetAllLeadsEndpoint);
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
                var request = _requestHelper.CreatePostRequest(Endpoints.GetTransactionByPeriodEndpoint, period);
                var response = _client.Execute<string>(request);
                var result = JsonConvert.DeserializeObject<List<TransactionBusinessModel>>(response.Data);
                transactionsList.Union(result);
            }
            return transactionsList;
        }

        public LeadOutputModel ChangeStatus(int leadId, Role status)
        {
            var endpoint = String.Format(Endpoints.ChangeStatusEndpoint, leadId, status);
            var request = _requestHelper.CreatePutRequest(endpoint);
            var response = _client.Execute<LeadOutputModel>(request);
            return response.Data;
        }
    }
}
