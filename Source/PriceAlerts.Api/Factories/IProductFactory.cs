using System;
using System.Threading.Tasks;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Factories
{
    public interface IProductFactory
    {
        Task<MonitoredProduct> CreateProduct(Uri uri);
    }
}