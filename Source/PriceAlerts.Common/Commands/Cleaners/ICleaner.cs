using System;

namespace PriceAlerts.Common.Commands.Cleaners
{
    internal interface ICleaner : ICommand
    {
        Uri CleanUrl(Uri originalUrl);
    }
}