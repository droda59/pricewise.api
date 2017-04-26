using System;

namespace PriceAlerts.Api.Models
{
    public class Deal
    {
        public decimal Price { get; set; }

        public string Title { get; set; }

        public string ProductUrl { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}