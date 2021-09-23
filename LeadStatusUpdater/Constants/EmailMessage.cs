using LeadStatusUpdater.Models;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace LeadStatusUpdater.Constants
{
    public class EmailMessage
    {
        public const string BestCrm = "Best CRM";
        public const string StatusChangedSubject = "Status changed";
        public const string StatusChangedBody = "You status has been changed to {0}.";
        public const string HappyBirthdaySubject = "Happy birthday";
        public const string HappyBirthdayBody = "Dear, {0} {1}! Happy Birthday from best CRM service!";
        public const string CycleFailedBody = "VSE SLOMALOSYA {0}";
        public const string CycleFailedSubject = "VSE PLOHO";

        public static EmailModel GetStatusChangedEmail(LeadOutputModel lead)
        {
            return new EmailModel
            {
                Subject = EmailMessage.StatusChangedSubject,
                Body = String.Format(EmailMessage.StatusChangedBody, lead.Role),
                MailAddresses = lead.Email,
                DisplayName = BestCrm,
                IsBodyHtml = false
            };
        }

        public static EmailModel GetBirthdayEmail(LeadOutputModel lead)
        {
            return new EmailModel
            {
                Subject = EmailMessage.HappyBirthdaySubject,
                Body = String.Format(EmailMessage.HappyBirthdayBody, lead.LastName, lead.FirstName),
                MailAddresses = lead.Email,
                DisplayName = BestCrm,
                IsBodyHtml = false
            };
        }

        public static EmailModel GetBadEmail(string exception)
        {
            return new EmailModel
            {
                Subject = EmailMessage.CycleFailedSubject,
                Body = string.Format(EmailMessage.CycleFailedBody, exception),
                DisplayName = "StatusUpdater",
                MailAddresses = "merymal2696@gmail.com"
            };
        }
    }
}
