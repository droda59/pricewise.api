using System;

namespace PriceAlerts.Common.Commands.Cleaners
{
    public interface ICleaner : ICommand
    {
        Uri CleanUrl(Uri originalUrl);
    }
}