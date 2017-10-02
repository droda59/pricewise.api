using System;
using System.Collections.Generic;

namespace PriceAlerts.Api.Models
{
    public class ProductInfo
    {
        public string OriginalUrl { get; set; }

        public string ProductUrl { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string ProductIdentifier { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}