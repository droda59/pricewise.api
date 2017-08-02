using System;
using System.Linq;
using System.Text.RegularExpressions;

using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.UrlCleaners.Sources
{
    public class AmazonCleaner : BaseCleaner, ICleaner
    {
        private readonly Regex _idExpression;

        public AmazonCleaner(AmazonSource source)
            : base(source)
        {
            this._idExpression = new Regex(@"[a-zA-Z0-9]{10}$", RegexOptions.Compiled);
        }

        public override Uri CleanUrl(Uri originalUrl)
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

                if (this._idExpression.IsMatch(segment))
                {
                    newUrl += segment;
                    break;
                }
            }

            return new Uri(this.Source.Domain, newUrl);
        }
    }
}