using System;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    public class BestBuyCleaner : BaseCleaner, ICleaner
    {
        private readonly BestBuySource _source;

        public BestBuyCleaner(BestBuySource source)
        {
            this._source = source;
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

                if (this._source.IdExpression.IsMatch(segment))
                {
                    newUrl += segment;
                    break;
                }
            }

            return new Uri(this._source.Domain, newUrl);
        }
    }
}