using System;

namespace PriceAlerts.Common.Commands.Cleaners
{
    internal class AnchorCleaner : ICleaner
    {
        public Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutAnchor = originalUrl.AbsoluteUri.Substring(0, originalUrl.AbsoluteUri.LastIndexOf("#", StringComparison.Ordinal));

            return new Uri(urlWithoutAnchor);
        }
    }
}