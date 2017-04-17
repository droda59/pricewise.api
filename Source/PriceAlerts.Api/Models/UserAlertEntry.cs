using System;

namespace PriceAlerts.Api.Models
{
    public class UserAlertEntry
    {
        public string Uri { get; set; }
        
        public decimal LastPrice { get; set; }

        public DateTime LastUpdate { get; set; }

        public bool IsDeleted { get; set; }
    }
}