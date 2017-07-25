using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Cleaners.Sources
{
    internal class CanadianTireCleaner : BaseCleaner, ICleaner
    {
        public CanadianTireCleaner()
            : base(new Uri("http://www.canadiantire.ca/"))
        {
        }

        public override Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;
            
            return urlWithoutQueryString;
        }
    }
}