using System;
using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.LinkManipulators
{
    internal class AmazonLinkManipulator : ICleaner, ILinkManipulator
    {
        private static string StoreId = "pricewise0d-20";

        private readonly AmazonSource _source;

        public AmazonLinkManipulator(AmazonSource source)
        {
            this._source = source;
        }

        public Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;
            var cleanUrl = urlWithoutQueryString.AbsoluteUri;
            var refIndex = cleanUrl.LastIndexOf("/ref=");
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

        public Uri ManipulateLink(Uri originalUrl)
        {
            var urlBuilder = new UriBuilder(originalUrl) { Query = $"tag={StoreId}" };

            return urlBuilder.Uri;
        }
    }
}