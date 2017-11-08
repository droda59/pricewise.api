using System;

namespace PriceAlerts.Api.Models
{
    public class UserAlertEntryDto
    {
        public string OriginalUrl { get; set; }

        public string ProductUrl { get; set; }

        public decimal LastPrice { get; set; }

        public string ProductIdentifier { get; set; }

        public bool IsDeleted { get; set; }
    }
}