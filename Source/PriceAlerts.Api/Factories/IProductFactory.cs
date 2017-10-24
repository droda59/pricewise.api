using System;
using System.Threading.Tasks;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Factories
{
    public interface IProductFactory
    {
        [LoggingDescription("Create product model")]
        Task<MonitoredProduct> CreateProduct(Uri uri);

        [LoggingDescription("Create updated product model")]
        Task<MonitoredProduct> CreateUpdatedProduct(Uri url);
    }
}