using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.CommandHandlers
{
    public interface ICommandHandler
    {
        Uri Domain { get; }

        Uri HandleCleanUrl(Uri url);

        Uri HandleManipulateUrl(Uri url);

        Task<SitePriceInfo> HandleGetInfo(Uri url);

        Task<IEnumerable<Uri>> HandleSearch(string searchTerm);
    }
}