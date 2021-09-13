using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Settings;
using Microsoft.Extensions.Options;
using RestSharp;
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

        public List<LeadOutputModel> GetAllLeads()
        {
            var adminToken = SignInByEmailAndPasswordReturnToken();
            var request = _requestHelper.CreateGetRequest(Endpoints.GetAllLeadsEndpoint, adminToken);
            var response = _client.Execute<List<LeadOutputModel>>(request);
            return response.Data;
        }


        public List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model)
        {
            var adminToken = SignInByEmailAndPasswordReturnToken();
            var request = _requestHelper.CreatePostRequest(Endpoints.GetTransactionByPeriodEndpoint, model, adminToken);
            var response = _client.Execute<List<AccountBusinessModel>>(request);
            return response.Data;
        }

        public LeadOutputModel ChangeStatus(int leadId, Role status)
        {
            var adminToken = SignInByEmailAndPasswordReturnToken();
            var endpoint = String.Format(Endpoints.ChangeStatusEndpoint, leadId, status);
            var request = _requestHelper.CreatePutRequest(endpoint, status, adminToken);
            var response = _client.Execute<LeadOutputModel>(request);
            return response.Data;
        }

        public string SignInByEmailAndPasswordReturnToken()
        {
            var postData = new AdminSignInModel { Email = _options.Value.AdminEmail, Password = _options.Value.AdminPassword };
            var request = _requestHelper.CreatePostRequest(Endpoints.SignInEndpoint, postData);
            var result = _client.Execute<string>(request).Data;
            return result;
        }
    }
}
