using System;

namespace PriceAlerts.Common.Models
{
    public class Deal
    {
        public decimal Price { get; set; }

        public string ProductId { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}