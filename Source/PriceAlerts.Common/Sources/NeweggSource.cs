using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceAlerts.Common.Sources
{
    public class NeweggSource// : ISource
    {
        public NeweggSource()
        {
            this.Domain = new Uri("https://www.newegg.ca/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public Uri Domain { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}
