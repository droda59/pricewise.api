using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

using PriceAlerts.Api.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.LinkManipulators
{
    internal class LegoLinkManipulator : EmptyQueryStringCleaner, ILinkManipulator
    {
        private static string Id = "6/oq6I8N2O0";
        private static string AdvertiserId = "13923";

        public LegoLinkManipulator(LegoSource source)
        {
            this.Source = source;
        }

        protected ISource Source { get; }

        public Uri ManipulateLink(Uri originalUrl)
        {
            var encodedLink = WebUtility.UrlEncode(originalUrl.AbsoluteUri);
            var deepLink = new Uri($"http://click.linksynergy.com/deeplink?id={Id}&mid={AdvertiserId}&murl={encodedLink}");

// https://click.linksynergy.com/deeplink?id=6/oq6I8N2O0&mid=13923&u1=pricewise&murl=https%3A%2F%2Fshop.lego.com%2Fen-CA%2FLondon-Bus-10258
            return deepLink;
        }
    }
}