using System.Collections.Generic;
using System.Linq;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Models
{
    public class ProductHistory
    {
        public ProductHistory()
        {
            this.PriceHistory = Enumerable.Empty<PriceChange>();
        }

        public string Title { get; set; }

        public string Url { get; set; }

        public IEnumerable<PriceChange> PriceHistory { get; internal set; }

        public override string ToString()
        {
            var title = this.Title.Length > 30 
                ? this.Title.Trim().Substring(0, 30) + "..." 
                : this.Title;
            
            return $"{this.Url}: {title}";
        }
    }
}