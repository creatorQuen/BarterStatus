namespace LeadStatusUpdater.Constants
{
    public class Endpoints
    {
        public const string GetTransactionByPeriodEndpoint = "/api/Account/by-period";
        public const string GetAllLeadsEndpoint = "/api/Lead";
        public const string GetAllLeadsByFilterEndpoint = "/api/Lead/filter";
        public const string ChangeStatusEndpoint = "/api/Lead/{id}/role/{role}";
        public const string SignInEndpoint = "api/auth/sign-in";
    }
}
