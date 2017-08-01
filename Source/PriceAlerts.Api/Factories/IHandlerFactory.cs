using System;

using PriceAlerts.Api.SourceHandlers;

namespace PriceAlerts.Api.Factories
{
    public interface IHandlerFactory
    {
        IHandler CreateHandler(Uri uri);
    }
}