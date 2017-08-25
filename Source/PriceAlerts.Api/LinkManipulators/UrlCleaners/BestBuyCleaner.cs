using System;
using System.Linq;
using System.Text.RegularExpressions;

using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.LinkManipulators.UrlCleaners
{
    internal class BestBuyCleaner : ICleaner
    {
        private readonly Regex _idExpression;
        private readonly ISource _source;

        public BestBuyCleaner(BestBuySource source)
        {
            this._source = source;
            this._idExpression = new Regex(@"[a-zA-Z0-9]{8}(.aspx)", RegexOptions.Compiled);
        }

        public Uri CleanUrl(Uri originalUrl)
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

            return new Uri(this._source.Domain, newUrl);
        }
    }
}