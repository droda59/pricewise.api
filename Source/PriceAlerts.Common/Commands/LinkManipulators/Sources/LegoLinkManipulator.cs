using System;
using System.Net;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Common.Commands.LinkManipulators.Sources
{
    public class LegoLinkManipulator : ILinkManipulator
    {
        private const string Id = "6/oq6I8N2O0";
        private const string AdvertiserId = "13923";

        [LoggingDescription("Manipulating URL")]
        public Uri ManipulateLink(Uri originalUrl)
        {
            var encodedLink = WebUtility.UrlEncode(originalUrl.AbsoluteUri);
            var deepLink = new Uri($"http://click.linksynergy.com/deeplink?id={Id}&mid={AdvertiserId}&murl={encodedLink}");

            return deepLink;
        }
    }
}