using System;

namespace PriceAlerts.Api.Models
{
    public class UserAlertEntryDto
    {
        public string Uri { get; set; }

        public string Title { get; set; }
        
        public decimal LastPrice { get; set; }

        public string ProductIdentifier { get; set; }

        public bool IsDeleted { get; set; }
    }
}