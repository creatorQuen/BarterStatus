namespace LeadStatusUpdater.Constants
{
    public class Endpoints
    {
        public static string GetTransactionByPeriodEndpoint = "/api/Account/by-period";
        public static string GetLeadsByBatchesEndpoint = "/api/lead/by-batches/cursorId/";
        public static string ChangeStatusEndpoint = "/api/Lead/{0}/role/{1}";
        public static string SignInEndpoint = "api/auth/sign-in";
    }
}
