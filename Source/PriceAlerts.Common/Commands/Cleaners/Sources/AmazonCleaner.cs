using System;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    internal class AmazonCleaner : ICleaner
    {
        private readonly AmazonSource _source;

        public AmazonCleaner(AmazonSource source)
        {
            this._source = source;
        }

        public Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;
            var cleanUrl = urlWithoutQueryString.AbsoluteUri;
            var refIndex = cleanUrl.LastIndexOf("/ref=", StringComparison.InvariantCulture);
            if (refIndex >= 0)
            {
                cleanUrl = cleanUrl.Substring(0, refIndex);
            }

            var newUrl = string.Empty;
            var urlSegments = new Uri(cleanUrl).Segments;
            foreach (var segment in urlSegments)
            {
                if (string.Equals(segment, "/", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(segment, "gp/", StringComparison.OrdinalIgnoreCase) 
                    || string.Equals(segment, "dp/", StringComparison.OrdinalIgnoreCase))
                {
                    newUrl += "dp/";
                    continue;
                }

                if (this._source.AsinExpression.IsMatch(segment))
                {
                    newUrl += segment;
                    break;
                }
            }

            return new Uri(this._source.Domain, newUrl);
        }
    }
}