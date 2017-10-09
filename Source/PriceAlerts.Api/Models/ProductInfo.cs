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

        public override string ToString()
        {
            var title = this.Title.Length > 30 
                ? this.Title.Trim().Substring(0, 30) + "..." 
                : this.Title;
            
            return $"{this.OriginalUrl}: {title}";
        }
    }
}