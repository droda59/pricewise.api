using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class RenaudBraySource : ISource
    {
        public RenaudBraySource()
        {
            this.Domain = new Uri("http://www.renaud-bray.com/");
            this.CustomHeaders = Enumerable.Empty<KeyValuePair<string, string>>();
            this.IdExpression = new Regex(@"[0-9]{7}$", RegexOptions.Compiled);
        }

        public Uri Domain { get; }
        
        public Regex IdExpression { get; }

        public IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}
