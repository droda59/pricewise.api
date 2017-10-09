using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.CommandHandlers
{
    public interface ICommandHandler
    {
        Uri Domain { get; }

        [LoggingDescription("Handling cleaning of URL")]
        Uri HandleCleanUrl(Uri url);

        [LoggingDescription("Handling manipulation of URL")]
        Uri HandleManipulateUrl(Uri url);

        [LoggingDescription("Handling inspection for product info")]
        Task<SitePriceInfo> HandleGetInfo(Uri url);

        [LoggingDescription("Handling search")]
        Task<IEnumerable<Uri>> HandleSearch(string searchTerm);
    }
}