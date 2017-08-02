using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.UrlCleaners.Sources
{
    internal class CarcajouCleaner : BaseCleaner, ICleaner
    {
        public CarcajouCleaner(CarcajouSource source)
            : base(source)
        {
        }

        public override Uri CleanUrl(Uri originalUrl)
        {
            StringValues sku = StringValues.Empty;
            var queryParameters = QueryHelpers.ParseQuery(originalUrl.Query);
            if (queryParameters.TryGetValue("prod_id", out sku))
            {
                var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty };
                var urlWithQueryString = QueryHelpers.AddQueryString(urlWithoutQueryString.Uri.AbsoluteUri, "prod_id", sku);

                return new Uri(urlWithQueryString);
            }

            return null;
        }
    }
}