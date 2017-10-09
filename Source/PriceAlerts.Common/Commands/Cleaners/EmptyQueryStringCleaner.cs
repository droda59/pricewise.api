using System;

namespace PriceAlerts.Common.Commands.Cleaners
{
    public class EmptyQueryStringCleaner : BaseCleaner, ICleaner
    {
        public override Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;

            return urlWithoutQueryString;
        }
    }
}