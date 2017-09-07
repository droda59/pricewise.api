using System;

namespace PriceAlerts.Common.Commands.Cleaners
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