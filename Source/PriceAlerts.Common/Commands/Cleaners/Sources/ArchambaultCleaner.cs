using System;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    internal class ArchambaultCleaner : ICleaner
    {
        public Uri CleanUrl(Uri originalUrl)
        {
            StringValues sku;
            var queryParameters = QueryHelpers.ParseQuery(originalUrl.Query);
            if (queryParameters.TryGetValue("id", out sku))
            {
                var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty };

                if (urlWithoutQueryString.Uri.AbsoluteUri.EndsWith("/"))
                {
                    var shortenAbsoluteUrl = urlWithoutQueryString.Uri.AbsoluteUri.Substring(0, urlWithoutQueryString.Uri.AbsoluteUri.Length - 2);
                    return new Uri(QueryHelpers.AddQueryString(shortenAbsoluteUrl, "id", sku));
                }
                
                return new Uri(QueryHelpers.AddQueryString(urlWithoutQueryString.Uri.AbsoluteUri, "id", sku));
            }

            return originalUrl;
        }
    }
}