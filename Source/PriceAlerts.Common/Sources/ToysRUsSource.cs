using System;
using System.Collections.Generic;
using System.Linq;

namespace PriceAlerts.Common.Sources
{
    public class ToysRUsSource : ISource
    {
        public ToysRUsSource()
        {
            this.Domain = new Uri("http://www.toysrus.ca/");
            this.CustomHeaders = new[] 
            {
                new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36")
            };
        }

        public Uri Domain { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}
