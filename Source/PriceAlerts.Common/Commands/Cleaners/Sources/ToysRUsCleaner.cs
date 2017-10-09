using System;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    public class ToysRUsCleaner : BaseCleaner, ICleaner
    {
        public override Uri CleanUrl(Uri originalUrl)
        {
            StringValues sku;
            var queryParameters = QueryHelpers.ParseQuery(originalUrl.Query);
            if (queryParameters.TryGetValue("productId", out sku))
            {
                var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty };
                var urlWithQueryString = QueryHelpers.AddQueryString(urlWithoutQueryString.Uri.AbsoluteUri, "productId", sku);

                return new Uri(urlWithQueryString);
            }

            return originalUrl;
        }
    }
}