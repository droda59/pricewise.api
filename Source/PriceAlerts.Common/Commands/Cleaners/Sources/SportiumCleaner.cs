using System;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    public class SportiumCleaner : BaseCleaner, ICleaner
    {
        private readonly SportiumSource _source;

        public SportiumCleaner(SportiumSource source)
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

                if (string.Equals(segment, "en/", StringComparison.OrdinalIgnoreCase) 
                    || string.Equals(segment, "fr/", StringComparison.OrdinalIgnoreCase))
                {
                    newUrl += segment;
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