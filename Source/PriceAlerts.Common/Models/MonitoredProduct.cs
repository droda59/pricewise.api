using System.Collections.Generic;

namespace PriceAlerts.Common.Models
{
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
    }
}