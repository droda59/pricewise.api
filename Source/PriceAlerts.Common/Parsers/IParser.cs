using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Parsers.Models;

namespace PriceAlerts.Common.Parsers
{
    public interface IParser
    {
        Task<SitePriceInfo> GetSiteInfo(Uri uri);

        Task<SitePriceInfo> GetSiteInfo(string uri);
    }
}