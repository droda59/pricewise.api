using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceAlerts.Common.Sources
{
    public class ArchambaultSource : ISource
    {
        public ArchambaultSource()
        {
            this.Domain = new Uri("http://www.archambault.ca/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public Uri Domain { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}
