using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PriceAlerts.Common.Parsers.Models;

namespace PriceAlerts.Api.SourceHandlers
{
    public interface IHandler
    {
        Uri Domain { get; }

        Uri HandleCleanUrl(Uri url);

        Task<SitePriceInfo> HandleParse(Uri url);

        Task<IEnumerable<Uri>> HandleSearch(string searchTerm);
    }
}