using System;

using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.LinkManipulators.UrlCleaners
{
    internal class StaplesCleaner : ICleaner
    {
        private readonly StaplesSource _source;

        public StaplesCleaner(StaplesSource source)
        {
            this._source = source;
        }

        public Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;
            var cleanUrl = urlWithoutQueryString.AbsoluteUri;

            var newUrl = string.Empty;
            var urlSegments = new Uri(cleanUrl).Segments;
            foreach (var segment in urlSegments)
            {
                if (string.Equals(segment, "/", StringComparison.OrdinalIgnoreCase))
                {
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