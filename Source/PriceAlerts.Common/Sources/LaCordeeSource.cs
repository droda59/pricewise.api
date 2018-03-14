using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceAlerts.Common.Sources
{
    public class LaCordeeSource : ISource
    {   
        public LaCordeeSource()
        {
            this.Domain = new Uri("https://www.lacordee.com/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public Uri Domain { get; }
        
        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}