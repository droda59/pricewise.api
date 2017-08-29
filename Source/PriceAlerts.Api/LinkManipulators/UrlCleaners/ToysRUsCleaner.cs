using System;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace PriceAlerts.Api.LinkManipulators.UrlCleaners
{
    internal class ToysRUsCleaner : ICleaner
    {
        public Uri CleanUrl(Uri originalUrl)
        {
            StringValues sku = StringValues.Empty;
            var queryParameters = QueryHelpers.ParseQuery(originalUrl.Query);
            if (queryParameters.TryGetValue("productId", out sku))
            {
                var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty };
                var urlWithQueryString = QueryHelpers.AddQueryString(urlWithoutQueryString.Uri.AbsoluteUri, "productId", sku);

                return new Uri(urlWithQueryString);
            }

            return null;
        }
    }
}