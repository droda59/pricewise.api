using System;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace PriceAlerts.Api.LinkManipulators.UrlCleaners
{
    internal class CarcajouCleaner : ICleaner
    {
        public Uri CleanUrl(Uri originalUrl)
        {
            StringValues sku;
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