using System;
using System.Linq;
using System.Text.RegularExpressions;

using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.LinkManipulators.UrlCleaners
{
    internal class StaplesCleaner : ICleaner
    {
        private readonly Regex _idExpression;
        private readonly ISource _source;

        public StaplesCleaner(StaplesSource source)
        {
            this._source = source;
            this._idExpression = new Regex(@"product_[0-9]+_", RegexOptions.Compiled);
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