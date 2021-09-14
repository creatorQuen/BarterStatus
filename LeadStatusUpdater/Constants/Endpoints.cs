namespace LeadStatusUpdater.Constants
{
    public class Endpoints
    {
        public static string GetTransactionByPeriodEndpoint = "/api/Account/by-period";
        public static string GetLeadsByFiltersEndpoint = "/api/lead/by-filters";
        public static string ChangeStatusEndpoint = "/api/Lead/{0}/role/{1}";
        public static string SignInEndpoint = "api/auth/sign-in";
    }
}
