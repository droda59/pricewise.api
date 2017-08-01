using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Parsers.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Parsers
{
    public interface IParser
    {
        Task<SitePriceInfo> GetSiteInfo(Uri url);

        ISource Source { get; }
    }
}