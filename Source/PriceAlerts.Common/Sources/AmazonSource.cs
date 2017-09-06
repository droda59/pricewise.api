using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class AmazonSource : ISource
    {
        public AmazonSource()
        {
            this.Domain = new Uri("https://www.amazon.ca/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
            this.AsinExpression = new Regex(@"[a-zA-Z0-9]{10}(/{0,1})$", RegexOptions.Compiled);
        }

        public Uri Domain { get; }
        
        public Regex AsinExpression { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}
