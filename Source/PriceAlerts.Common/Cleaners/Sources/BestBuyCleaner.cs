using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Cleaners.Sources
{
    internal class BestBuyCleaner : BaseCleaner, ICleaner
    {
        private readonly Regex _idExpression;

        public BestBuyCleaner()
            : base(new Uri("http://www.bestbuy.ca/"))
        {
            this._idExpression = new Regex(@"[a-zA-Z0-9]{8}(.aspx)", RegexOptions.Compiled);
        }

        public override Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;

            var newUrl = string.Empty;
            var urlSegments = urlWithoutQueryString.Segments;
            foreach (var segment in urlSegments)
            {
                if (string.Equals(segment, "/", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(segment, "en-ca/", StringComparison.OrdinalIgnoreCase) 
                    || string.Equals(segment, "fr-ca/", StringComparison.OrdinalIgnoreCase))
                {
                    newUrl += segment;
                    continue;
                }

                if (string.Equals(segment, "product/", StringComparison.OrdinalIgnoreCase))
                {
                    newUrl += segment;
                    newUrl += "-/";
                    continue;
                }

                if (this._idExpression.IsMatch(segment))
                {
                    newUrl += segment;
                    break;
                }
            }

            return new Uri(this.Domain, newUrl);
        }
    }
}