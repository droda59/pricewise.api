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
            if (string.IsNullOrEmpty(this.Id))
            {
                return "New product";
            }

            var title = this.Title.Length > 30 
                ? this.Title.Trim().Substring(0, 30) + "..." 
                : this.Title;
            
            return $"{this.Id}: {title}";
        }
    }
}