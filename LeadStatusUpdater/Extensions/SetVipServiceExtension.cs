using LeadStatusUpdater.Models;
using LeadStatusUpdater.Services;
using System;
using System.Collections.Generic;

namespace LeadStatusUpdater.Extensions
{
    public static class SetVipServiceExtension
    {
        private const string _dateFormatWithMinutesAndSeconds = "dd.MM.yyyy HH:mm";

        public static List<AccountBusinessModel> GetTransactionsByPeriod(this ISetVipService service, IRequestsSender requests, 
            string _adminToken, int days, int accountId)
        {
            var period = new TimeBasedAcquisitionInputModel
            {
                To = DateTime.Now.ToString(_dateFormatWithMinutesAndSeconds),
                From = DateTime.Now.AddDays(-days).ToString(_dateFormatWithMinutesAndSeconds),
                AccountId = accountId
            };

            return requests.GetTransactionsByPeriod(period, _adminToken);
        }


    }

}
