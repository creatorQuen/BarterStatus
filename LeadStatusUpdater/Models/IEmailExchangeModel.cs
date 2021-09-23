using System.Collections.Generic;
using System.Net.Mail;

namespace MailExchange
{
    public interface IMailExchangeModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string DisplayName { get; set; }
        public string MailAddresses { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Base64String { get; set; }
    }
}
