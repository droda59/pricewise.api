using System;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Common.Commands.LinkManipulators
{
    public class CJLinkManipulator : ILinkManipulator
    {
        private const string WebsiteId = "8408170";

        [LoggingDescription("Manipulating URL")]
        public Uri ManipulateLink(Uri originalUrl)
        {
            var deepLink = new Uri($"http://www.qksrv.net/links/{WebsiteId}/type/am/{originalUrl.AbsoluteUri}");

            return deepLink;
        }
    }
}