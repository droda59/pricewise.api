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

        public IEnumerable<PriceChange> PriceHistory { get; internal set; }
    }
}