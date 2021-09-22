using LeadStatusUpdater.Constants;
using LeadStatusUpdater.Models;
using LeadStatusUpdater.Services;
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

        public List<LeadOutputModel> GetRegularAndVipLeads(int lastLeadId)
        {
            var endpoint = $"{Endpoints.GetLeadsByBatchesEndpoint}{lastLeadId}";
            var request = _requestHelper.CreateGetRequest(endpoint, SetVipService.AdminToken);
            return SendRequest<List<LeadOutputModel>>(endpoint, request);
        }

        public List<TransactionOutputModel> GetTransactionsByPeriod(List<int> accountIds)
        {
            var endpoint = Endpoints.GetTransactionsByTwoMonthAndAccountIds;
            var request = _requestHelper.CreatePostRequest(endpoint, accountIds, SetVipService.AdminToken);
            return SendRequest<List<TransactionOutputModel>>(endpoint, request);
        }

        public int ChangeStatus(List<LeadIdAndRoleInputModel> model)
        {
            var endpoint = Endpoints.ChangeRoleEndpoint;
            var request = _requestHelper.CreatePutRequest(endpoint, model, SetVipService.AdminToken);
            return SendRequest<int>(endpoint, request);
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
                if (i != _retryCount) Thread.Sleep(_retryTimeout);
            }
            throw new Exception($"{LogMessages.CrmNotResponding}");
        }

        private T SendRequest<T>(string endpoint, IRestRequest request)
        {
            IRestResponse<T> response;
            for (int i = 1; i <= _retryCount; i++)
            {
                response = _client.Execute<T>(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.Data;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Log.Warning($"{LogMessages.RequestResult}", endpoint, response.StatusCode);
                    SetVipService.AdminToken = GetAdminToken();
                    i--;
                    continue;
                }
                var error = response.ErrorMessage == default ? response.Content : response.ErrorMessage;
                Log.Error($"{LogMessages.RequestFailed}", i, endpoint, error);
                if (i != _retryCount) Thread.Sleep(_retryTimeout);
            }
            throw new Exception($"{LogMessages.CrmNotResponding}");
        }
    }
}
