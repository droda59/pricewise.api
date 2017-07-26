using System;

namespace PriceAlerts.Common.Cleaners.Sources
{
    internal class IndigoCleaner : BaseCleaner, ICleaner
    {
        public IndigoCleaner()
            : base(new Uri("https://www.chapters.indigo.ca/"))
        {
        }

        public override Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;

            return urlWithoutQueryString;
        }
    }
}