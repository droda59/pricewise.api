using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace PriceAlerts.Common.Models
{
    [BsonIgnoreExtraElements]
    public class MonitoredProduct : Document
    {
        public MonitoredProduct()
        {
            this.PriceHistory = new List<PriceChange>();
        }

        public string ProductIdentifier { get; set; }

        public string Uri { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }
        
        public IList<PriceChange> PriceHistory { get; set; }

        public override string ToString()
        {
            var title = this.Title;
            if (title.Length > 30)
            {
                title = title.Trim().Substring(0, 30) + "...";
            }
            
            return string.IsNullOrEmpty(this.Id) 
                ? "New product" 
                : $"{this.Id}: {title}";
        }
    }
}