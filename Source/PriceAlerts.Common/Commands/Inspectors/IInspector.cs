using System;
using System.Threading.Tasks;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Commands.Inspectors
{
    internal interface IInspector : ICommand
    {
        Task<SitePriceInfo> GetSiteInfo(Uri url);
    }
}