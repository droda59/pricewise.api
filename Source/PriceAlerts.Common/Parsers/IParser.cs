using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Parsers.Models;

namespace PriceAlerts.Common.Parsers
{
    public interface IParser
    {
        Uri Domain { get; }

        Task<SitePriceInfo> GetSiteInfo(Uri uri);
    }
}