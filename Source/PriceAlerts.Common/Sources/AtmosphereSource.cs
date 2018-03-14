using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class AtmosphereSource : ISource
    {   
        public AtmosphereSource()
        {
            this.Domain = new Uri("https://www.atmosphere.ca/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
            this.IdExpression = new Regex(@"[0-9]{9}", RegexOptions.Compiled);
        }

        public Uri Domain { get; }
        
        public Regex IdExpression { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}