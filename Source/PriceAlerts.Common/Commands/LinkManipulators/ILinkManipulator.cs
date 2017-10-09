using System;

namespace PriceAlerts.Common.Commands.LinkManipulators
{
    public interface ILinkManipulator : ICommand
    {
        Uri ManipulateLink(Uri originalUrl);
    }
}