using RestSharp;

namespace LeadStatusUpdater.Requests
{
    public static class RequestHelperExtension
    {
        public static IRestRequest CreateGetRequest(this RequestHelper requestHelper, string endPoint, string token = default)
        {
            var request = requestHelper.CreateRequest(Method.GET, endPoint);
            if (!string.IsNullOrEmpty(token)) { request.Authorize(token); }

            return request;
        }

        public static IRestRequest CreatePostRequest<T>(this RequestHelper requestHelper, string endPoint, T postData, string token = default)
        {
            var request = requestHelper.CreateRequest(Method.POST, endPoint);
            request.AddJsonBody(postData);
            if (!string.IsNullOrEmpty(token)) { request.Authorize(token); }

            return request;
        }

        public static IRestRequest CreatePutRequest<T>(this RequestHelper requestHelper, string endPoint, T postData, string token = default)
        {
            var request = requestHelper.CreateRequest(Method.PUT, endPoint);
            request.AddJsonBody(postData);
            if (!string.IsNullOrEmpty(token)) { request.Authorize(token); }

            return request;
        }

        private static void Authorize(this IRestRequest request, string token)
        {
            request.AddHeader("Authorization", $"Bearer {token}");
        }
    }
}
