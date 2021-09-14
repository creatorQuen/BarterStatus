using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Settings;
using Microsoft.Extensions.Options;
using RestSharp;
using Serilog;
using System.Collections.Generic;

namespace LeadStatusUpdater.Requests
{
    public class RequestsSender : IRequestsSender
    {
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;
        private readonly IOptions<AppSettings> _options;

        public RequestsSender(IOptions<AppSettings> options)
        {
            _options = options;
            _client = new RestClient(_options.Value.ConnectionString);
            _requestHelper = new RequestHelper();
        }

        public List<LeadOutputModel> GetRegularAndVipLeads(string adminToken, int cursor)
        {
            var endpoint = $"{Endpoints.GetLeadsByBatchesEndpoint}{cursor}";
            var request = _requestHelper.CreateGetRequest(endpoint, adminToken);
            IRestResponse<List<LeadOutputModel>> response;
            do
            {
                response = _client.Execute<List<LeadOutputModel>>(request);
                Log.Information($"{LogMessages.RequestResult}", Endpoints.GetLeadsByBatchesEndpoint, response.StatusCode);
            }
            while (!response.IsSuccessful);
            return response.Data;
        }

        public List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model, string adminToken)
        {
            var request = _requestHelper.CreatePostRequest(Endpoints.GetTransactionByPeriodEndpoint, model, adminToken);
            IRestResponse<List<AccountBusinessModel>> response;
            do
            {
                response = _client.Execute<List<AccountBusinessModel>>(request);
                Log.Information($"{LogMessages.RequestResult}", Endpoints.GetTransactionByPeriodEndpoint, response.StatusCode);
            }
            while (!response.IsSuccessful);
            return response.Data;
        }

        public LeadOutputModel ChangeStatus(int leadId, Role status, string adminToken)
        {
            var endpoint = string.Format(Endpoints.ChangeStatusEndpoint, leadId, status);
            var request = _requestHelper.CreatePutRequest(endpoint, status, adminToken);
            var response = _client.Execute<LeadOutputModel>(request);
            return response.Data;
        }

        public string GetAdminToken()
        {
            var postData = new AdminSignInModel { Email = _options.Value.AdminEmail, Password = _options.Value.AdminPassword };
            var request = _requestHelper.CreatePostRequest(Endpoints.SignInEndpoint, postData);
            IRestResponse<string> response;
            do
            {
                response = _client.Execute<string>(request);
                Log.Information($"{LogMessages.RequestResult}", Endpoints.SignInEndpoint, response.StatusCode);
            }
            while (!response.IsSuccessful);
            return response.Data;
        }
    }
}
