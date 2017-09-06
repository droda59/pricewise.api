using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class BestBuySource : ISource
    {   
        public BestBuySource()
        {
            this.Domain = new Uri("http://www.bestbuy.ca/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
            this.IdExpression = new Regex(@"[a-zA-Z0-9]{8}(.aspx)", RegexOptions.Compiled);
        }

        public Uri Domain { get; }
        
        public Regex IdExpression { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}
