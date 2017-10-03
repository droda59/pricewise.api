using System;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    internal class TigerDirectCleaner : ICleaner
    {
        public Uri CleanUrl(Uri originalUrl)
        {
            StringValues sku;
            var queryParameters = QueryHelpers.ParseQuery(originalUrl.Query);
            if (queryParameters.TryGetValue("EdpNo", out sku))
            {
                var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty };
                var urlWithQueryString = QueryHelpers.AddQueryString(urlWithoutQueryString.Uri.AbsoluteUri, "EdpNo", sku);

                return new Uri(urlWithQueryString);
            }
            
            if (queryParameters.TryGetValue("sku", out sku))
            {
                var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty };
                var urlWithQueryString = QueryHelpers.AddQueryString(urlWithoutQueryString.Uri.AbsoluteUri, "sku", sku);

                return new Uri(urlWithQueryString);
            }
            
            return originalUrl;
        }
    }
}