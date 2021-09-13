namespace LeadStatusUpdater.Services
{
    public interface IConverterService
    {
        decimal ConvertAmount(string senderCurrency, string recipientCurrency, decimal amount);
    }
}
