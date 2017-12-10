using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class SportiumSource : ISource
    {   
        public SportiumSource()
        {
            this.Domain = new Uri("https://www.sportium.ca/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
            this.IdExpression = new Regex(@"([a-zA-Z0-9]+\\-)+[0-9]{6}", RegexOptions.Compiled);
        }

        public Uri Domain { get; }
        
        public Regex IdExpression { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}