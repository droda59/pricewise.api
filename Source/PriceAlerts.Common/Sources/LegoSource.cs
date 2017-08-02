using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceAlerts.Common.Sources
{
    public class LegoSource : ISource
    {
        public LegoSource()
        {
            this.Domain = new Uri("https://shop.lego.com/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public Uri Domain { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}
