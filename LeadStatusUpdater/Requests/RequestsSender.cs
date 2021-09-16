using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Enums;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Settings;
using Microsoft.Extensions.Options;
using RestSharp;
using Serilog;
using System;
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
        private const int _retryCount = 3;
        private const int _retryTimeout = 10_000;

        public RequestsSender(IOptions<AppSettings> options)
        {
            _options = options;
            _client = new RestClient(_options.Value.ConnectionString);
            _requestHelper = new RequestHelper();
        }

        public List<LeadOutputModel> GetRegularAndVipLeads(string adminToken, int lastLeadIs)
        {
            var endpoint = $"{Endpoints.GetLeadsByBatchesEndpoint}{lastLeadIs}";
            IRestResponse<List<LeadOutputModel>> response;

            for (int i = 1; i <= _retryCount; i++)
            {
                var request = _requestHelper.CreateGetRequest(endpoint, adminToken);
                response = _client.Execute<List<LeadOutputModel>>(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Log.Information($"{LogMessages.RequestResult}", endpoint, response.StatusCode);
                    return response.Data;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Log.Warning($"{LogMessages.RequestResult}", endpoint, response.StatusCode);
                    adminToken = GetAdminToken();
                    i--;
                    continue;
                }
                var error = response.ErrorMessage == default ? response.Content : response.ErrorMessage;
                Log.Error($"{LogMessages.RequestFailed}", i, endpoint, error);
                if (i != _retryCount - 1) Thread.Sleep(_retryTimeout);
            }
            throw new Exception($"{LogMessages.CrmNotResponding}");
        }

        public List<AccountBusinessModel> GetTransactionsByPeriod(TimeBasedAcquisitionInputModel model, string adminToken)
        {
            var endpoint = Endpoints.GetTransactionByPeriodEndpoint;
            IRestResponse<List<AccountBusinessModel>> response;

            for (int i = 1; i <= _retryCount; i++)
            {
                var request = _requestHelper.CreatePostRequest(endpoint, model, adminToken);
                response = _client.Execute<List<AccountBusinessModel>>(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //Log.Information($"{LogMessages.RequestResult}", endpoint, response.StatusCode);
                    return response.Data;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Log.Warning($"{LogMessages.RequestResult}", endpoint, response.StatusCode);
                    adminToken = GetAdminToken();
                    i--;
                    continue;
                }
                var error = response.ErrorMessage == default ? response.Content : response.ErrorMessage;
                Log.Error($"{LogMessages.RequestFailed}", i, endpoint, error);
                if (i != _retryCount - 1) Thread.Sleep(_retryTimeout);
            }
            throw new Exception($"{LogMessages.CrmNotResponding}");
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
            var endpoint = Endpoints.SignInEndpoint;
            var postData = new AdminSignInModel { Email = _options.Value.AdminEmail, Password = _options.Value.AdminPassword };
            IRestResponse<string> response;

            for (int i = 1; i <= _retryCount; i++)
            {
                var request = _requestHelper.CreatePostRequest(endpoint, postData);
                response = _client.Execute<string>(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Log.Information($"{LogMessages.NewTokenGenerated}");
                    return response.Data;
                }
                var error = response.ErrorMessage == default ? response.Content : response.ErrorMessage;
                Log.Error($"{LogMessages.RequestFailed}", i, endpoint, error);
                if (i != _retryCount - 1) Thread.Sleep(_retryTimeout);
            }
            throw new Exception($"{LogMessages.CrmNotResponding}");
        }

    }
}
