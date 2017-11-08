using System;

namespace PriceAlerts.Api.Models
{
    public class DealDto
    {
        public decimal Price { get; set; }

        public string OriginalUrl { get; set; }

        public string ProductUrl { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}