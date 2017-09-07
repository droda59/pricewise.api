using System;
using PriceAlerts.Common.SourceHandlers;

namespace PriceAlerts.Common.Factories
{
    public interface IHandlerFactory
    {
        IHandler CreateHandler(Uri uri);
    }
}