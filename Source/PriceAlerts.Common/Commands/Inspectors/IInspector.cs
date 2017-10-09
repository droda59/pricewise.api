using System;
using System.Threading.Tasks;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Commands.Inspectors
{
    public interface IInspector : ICommand
    {
        Task<SitePriceInfo> GetSiteInfo(Uri url);
    }
}