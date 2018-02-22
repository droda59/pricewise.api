using System.Collections.Generic;

namespace PriceAlerts.PriceCheckJob.Emails
{
    public class EmailInformation
    {
        public string RecipientAddress { get; set; }
            
        public string TemplateName { get; set; }

        public IDictionary<string, string> Parameters { get; set; }
    }
}