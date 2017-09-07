using System;

namespace PriceAlerts.Common.Commands.LinkManipulators.Sources
{
    internal class AmazonLinkManipulator : ILinkManipulator
    {
        private const string StoreId = "pricewise0d-20";

        public Uri ManipulateLink(Uri originalUrl)
        {
            var urlBuilder = new UriBuilder(originalUrl) { Query = $"tag={StoreId}" };

            return urlBuilder.Uri;
        }
    }
}