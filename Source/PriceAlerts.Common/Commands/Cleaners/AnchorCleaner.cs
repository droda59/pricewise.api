using System;

namespace PriceAlerts.Common.Commands.Cleaners
{
    public class AnchorCleaner : BaseCleaner, ICleaner
    {
        public override Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutAnchor = originalUrl.AbsoluteUri.Substring(0, originalUrl.AbsoluteUri.LastIndexOf("#", StringComparison.Ordinal));

            return new Uri(urlWithoutAnchor);
        }
    }
}