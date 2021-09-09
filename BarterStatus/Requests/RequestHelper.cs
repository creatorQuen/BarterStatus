using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarterStatus.Requests
{
    public class RequestHelper
    {
        public IRestRequest CreateRequest(Method httpMethod, string endPoint)
        {
            return new RestRequest(endPoint, httpMethod);
        }
    }
}
