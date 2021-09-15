using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Settings;
using Microsoft.Extensions.Options;
using RestSharp;
using Serilog;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace LeadStatusUpdater.Requests
{
    public class RequestsSender : IRequestsSender
    {
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;
        private readonly IOptions<AppSettings> _options;
        private int _retryCount;

        public RequestsSender(IOptions<AppSettings> options)
        {
            _options = options;
            _client = new RestClient(_options.Value.ConnectionString);
            _requestHelper = new RequestHelper();
        }

        public List<LeadOutputModel> GetRegularAndVipLeads(string adminToken, int cursor)
        {
            _retryCount = 0;
            var endpoint = $"{Endpoints.GetLeadsByBatchesEndpoint}{cursor}";
            IRestResponse<List<LeadOutputModel>> response;
            do
            {
                if (_retryCount > 3) Thread.Sleep(30000);
                var request = _requestHelper.CreateGetRequest(endpoint, adminToken);
                response = _client.Execute<List<LeadOutputModel>>(request);
                Log.Information($"{LogMessages.RequestResult}", Endpoints.GetLeadsByBatchesEndpoint, response.StatusCode);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    adminToken = GetAdminToken();
                    Log.Information(LogMessages.NewTokenGenerated);
                }
                _retryCount++;
            }
            while (!response.IsSuccessful);
            return response.Data;
        }

        public List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model, string adminToken)
        {
            _retryCount = 0;
            IRestResponse<List<AccountBusinessModel>> response;
            do
            {
                if (_retryCount > 3) Thread.Sleep(30000);
                var request = _requestHelper.CreatePostRequest(Endpoints.GetTransactionByPeriodEndpoint, model, adminToken);
                response = _client.Execute<List<AccountBusinessModel>>(request);
                Log.Information($"{LogMessages.RequestResult}", Endpoints.GetTransactionByPeriodEndpoint, response.StatusCode);
                if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    adminToken = GetAdminToken();
                    Log.Information(LogMessages.NewTokenGenerated); //change to const var
                }
                _retryCount++;
            }
            while (!response.IsSuccessful);
            return response.Data;
        }

        public LeadOutputModel ChangeStatus(int leadId, Role status, string adminToken) //change
        {
            var endpoint = string.Format(Endpoints.ChangeStatusEndpoint, leadId, status);
            var request = _requestHelper.CreatePutRequest(endpoint, status, adminToken);
            var response = _client.Execute<LeadOutputModel>(request);
            return response.Data;
        }

        public string GetAdminToken()
        {
            _retryCount = 0;
            var postData = new AdminSignInModel { Email = _options.Value.AdminEmail, Password = _options.Value.AdminPassword };
            var request = _requestHelper.CreatePostRequest(Endpoints.SignInEndpoint, postData);
            IRestResponse<string> response;
            do
            {
                if (_retryCount > 3) Thread.Sleep(30000);
                response = _client.Execute<string>(request);
                Log.Information($"{LogMessages.RequestResult}", Endpoints.SignInEndpoint, response.StatusCode);
                _retryCount++;
            }
            while (!response.IsSuccessful);
            return response.Data;
        }

    }
}
