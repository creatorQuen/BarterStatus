namespace LeadStatusUpdater.Constants
{
    public class LogMessages
    {
        public static string VipStatusGiven = "Vip status was given to Lead: Id: {0}, {1} {2} {3}, Email: {4}"; 
        public static string VipStatusTaken = "Vip status was taken from Lead: Id: {0}, {1} {2} {3}, Email: {4}"; 
        public static string RequestResult = "Request to endpoint {0} resulted with status code {1}"; 
        public static string NewTokenGenerated = "Token was generated";
        public static string RequestFailed = "Request № {0} to endpoint {1} failed: {2}";
        public static string RatesNotProvided = "Rates were not provided";
        public static string CrmNotResponding = "Connection with CRM was not established";
    }
}
