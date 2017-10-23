using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class HomeDepotSource : ISource
    {
        public HomeDepotSource()
        {
            this.Domain = new Uri("https://www.homedepot.ca/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
            this.SkuExpression = new Regex(@"[0-9]{10}$", RegexOptions.Compiled);
        }
        
        public Regex SkuExpression { get; }

        public Uri Domain { get; }
        
        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}