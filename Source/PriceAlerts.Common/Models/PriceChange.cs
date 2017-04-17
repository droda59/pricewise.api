using System;

namespace PriceAlerts.Common.Models
{
    public class PriceChange
    {
        public decimal Price { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}