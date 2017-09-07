using System;

namespace PriceAlerts.Common.Commands.LinkManipulators
{
    internal interface ILinkManipulator : ICommand
    {
        Uri ManipulateLink(Uri originalUrl);
    }
}