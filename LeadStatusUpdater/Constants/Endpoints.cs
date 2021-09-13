namespace LeadStatusUpdater.Constants
{
    public class Endpoints
    {
        public const string GetTransactionByPeriodEndpoint = "/api/Account/by-period";
        public const string GetAllLeadsEndpoint = "/api/Lead";
        public const string GetAllLeadsByFilterEndpoint = "/api/Lead/filter";
        public const string ChangeStatusEndpoint = "/api/Lead/{0}/role/{1}";
        public const string SignInEndpoint = "api/auth/sign-in";
    }
}
