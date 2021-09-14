using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Settings;
using Microsoft.Extensions.Options;
using RestSharp;
using Serilog;
using System;
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

        public List<LeadOutputModel> GetRegularAndVipLeads(string adminToken)
        {
            var filterInputModel = new LeadFiltersInputModel
            { Role = new List<int> { (int)Role.Regular, (int)Role.Vip } };
            var request = _requestHelper.CreatePostRequest(Endpoints.GetLeadsByFiltersEndpoint, filterInputModel, adminToken);
            request.Timeout = 300000;
            dynamic response;
            do
            {
                response = _client.Execute<List<LeadOutputModel>>(request);
                Log.Information($"{LogMessages.RequestResult}", Endpoints.GetLeadsByFiltersEndpoint, response.StatusCode);
            }
            while (!response.IsSuccessful);
            return response.Data;
        }

        public List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model, string adminToken)
        {
            var request = _requestHelper.CreatePostRequest(Endpoints.GetTransactionByPeriodEndpoint, model, adminToken);
            dynamic response;
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
            var endpoint = ($"{Endpoints.ChangeStatusEndpoint}", leadId, status).ToString();
            var request = _requestHelper.CreatePutRequest(endpoint, status, adminToken);
            var response = _client.Execute<LeadOutputModel>(request);
            return response.Data;
        }

        public string GetAdminToken()
        {
            var postData = new AdminSignInModel { Email = _options.Value.AdminEmail, Password = _options.Value.AdminPassword };
            var request = _requestHelper.CreatePostRequest(Endpoints.SignInEndpoint, postData);
            dynamic response;
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
