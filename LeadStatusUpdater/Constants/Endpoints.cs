namespace LeadStatusUpdater.Constants
{
    public class Endpoints
    {
        public static string GetTransactionsByTwoMonthAndAccountIds = "/api/account/by-accountIds";
        public static string GetLeadsByBatchesEndpoint = "/api/lead/by-batches/cursorId/";
        public static string ChangeRoleEndpoint = "/api/Lead/change-role-leads";
        public static string SignInEndpoint = "api/auth/sign-in";
    }
}
