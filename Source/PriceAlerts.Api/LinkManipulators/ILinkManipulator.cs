using System;

using PriceAlerts.Api.LinkManipulators.UrlCleaners;

namespace PriceAlerts.Api.LinkManipulators
{
    public interface ILinkManipulator : ICleaner
    {
        Uri ManipulateLink(Uri originalUrl);
    }
}