using System;
using PriceAlerts.Common.CommandHandlers;

namespace PriceAlerts.Common.Factories
{
    public interface IHandlerFactory
    {
        ICommandHandler CreateHandler(Uri uri);
    }
}