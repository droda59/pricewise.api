using System;

using PriceAlerts.Api.LinkManipulators.UrlCleaners;

namespace PriceAlerts.Api.LinkManipulators
{
    public interface ILinkManipulator
    {
        Uri ManipulateLink(Uri originalUrl);
    }
}