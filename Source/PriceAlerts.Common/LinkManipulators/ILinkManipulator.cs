using System;

namespace PriceAlerts.Common.LinkManipulators
{
    public interface ILinkManipulator
    {
        Uri ManipulateLink(Uri originalUrl);
    }
}