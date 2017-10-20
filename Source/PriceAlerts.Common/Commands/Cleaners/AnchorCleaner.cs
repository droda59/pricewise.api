using System;

namespace PriceAlerts.Common.Commands.Cleaners
{
    public class AnchorCleaner : BaseCleaner, ICleaner
    {
        public override Uri CleanUrl(Uri originalUrl)
        {
            var lastHash = originalUrl.AbsoluteUri.LastIndexOf("#", StringComparison.Ordinal);
            if (lastHash > -1)
            {
                var urlWithoutAnchor = originalUrl.AbsoluteUri.Substring(0, lastHash);

                return new Uri(urlWithoutAnchor);
            }

            return originalUrl;
        }
    }
}