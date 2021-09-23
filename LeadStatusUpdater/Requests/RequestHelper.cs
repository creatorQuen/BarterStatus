using RestSharp;
using System.Net;

namespace LeadStatusUpdater.Requests
{
    public class RequestHelper
    {
        public IRestRequest CreateRequest(Method httpMethod, string endPoint)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            return new RestRequest(endPoint, httpMethod);
        }
    }
}
