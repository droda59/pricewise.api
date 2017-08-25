using System;
using System.Net;

using Microsoft.AspNetCore.WebUtilities;

using PriceAlerts.Api.LinkManipulators.UrlCleaners;

namespace PriceAlerts.Api.LinkManipulators
{
    internal class LegoLinkManipulator : EmptyQueryStringCleaner, ILinkManipulator
    {
        private static string Id = "6/oq6I8N2O0";
        private static string AdvertiserId = "13923";

        public Uri ManipulateLink(Uri originalUrl)
        {
            var encodedLink = WebUtility.UrlEncode(originalUrl.AbsoluteUri);
            var deepLink = new Uri($"http://click.linksynergy.com/deeplink?id={Id}&mid={AdvertiserId}&murl={encodedLink}");

            return deepLink;
        }
    }
}