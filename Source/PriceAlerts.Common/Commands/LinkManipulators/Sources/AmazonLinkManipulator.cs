using System;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Common.Commands.LinkManipulators.Sources
{
    public class AmazonLinkManipulator : ILinkManipulator
    {
        private const string StoreId = "";

        [LoggingDescription("Manipulating URL")]
        public Uri ManipulateLink(Uri originalUrl)
        {
            var urlBuilder = new UriBuilder(originalUrl) { Query = $"tag={StoreId}" };

            return urlBuilder.Uri;
        }
    }
}
