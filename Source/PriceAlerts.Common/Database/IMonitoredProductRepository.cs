using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IProductRepository
    {
        [LoggingDescription("Get all products from database")]
        Task<IEnumerable<MonitoredProduct>> GetAllAsync();

        [LoggingDescription("Get all products from database")]
        Task<IEnumerable<MonitoredProduct>> GetAllByProductIdentifierAsync(string productIdentifier);
        
        [LoggingDescription("Get product from database")]
        Task<MonitoredProduct> GetAsync(string id);

        [LoggingDescription("Get product from database")]
        Task<MonitoredProduct> GetByUrlAsync(string url);

        [LoggingDescription("Update product in database")]
        Task<MonitoredProduct> UpdateAsync(string id, MonitoredProduct data);

        [LoggingDescription("Create new product in database")]
        Task<MonitoredProduct> InsertAsync(MonitoredProduct data);
    }
}