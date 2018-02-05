using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class _401GamesSource : ISource
    {
        public _401GamesSource()
        {
            this.Domain = new Uri("https://store.401games.ca");
            this.CustomHeaders = new[] 
            {
                new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36")
            };
            this.ProductExpression = new Regex("products/.*", RegexOptions.Compiled);
        }

        public Uri Domain { get; }
        
        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }

        public Regex ProductExpression { get; }
    }
}