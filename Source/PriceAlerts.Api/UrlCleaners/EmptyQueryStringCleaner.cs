using System;

namespace PriceAlerts.Api.UrlCleaners
{
    internal class EmptyQueryStringCleaner : ICleaner
    {
        public Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;

            return urlWithoutQueryString;
        }
    }
}