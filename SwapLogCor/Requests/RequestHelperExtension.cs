using RestSharp;

namespace SwapLogCor.Requests
{
    public static class RequestHelperExtension
    {
        public static IRestRequest CreateGetRequest(this RequestHelper requestHelper, string endPoint)
        {
            return requestHelper.CreateRequest(Method.GET, endPoint);
        }

        public static IRestRequest CreatePostRequest<T>(this RequestHelper requestHelper, string endPoint, T postData)
        {
            var request = requestHelper.CreateRequest(Method.POST, endPoint);
            return request.AddJsonBody(postData);
        }

        public static IRestRequest CreatePutRequest(this RequestHelper requestHelper, string endPoint)
        {
            return requestHelper.CreateRequest(Method.PUT, endPoint);
        }
    }
}
