using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class StaplesSource : ISource
    {
        public StaplesSource()
        {
            this.Domain = new Uri("https://www.staples.ca/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
            this.IdExpression = new Regex(@"product_[0-9]+_", RegexOptions.Compiled);
        }

        public Uri Domain { get; }
        
        public Regex IdExpression { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}
