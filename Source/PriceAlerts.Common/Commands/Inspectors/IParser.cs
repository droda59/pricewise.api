using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Commands.Inspectors
{
    public interface IParser : ICommand 
    {
        Task<SitePriceInfo> Parse(Uri url);
    }
}