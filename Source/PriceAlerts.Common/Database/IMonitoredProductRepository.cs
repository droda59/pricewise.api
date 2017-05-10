using System.Collections.Generic;
using System.Threading.Tasks;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IProductRepository
    {
        Task<IEnumerable<MonitoredProduct>> GetAllAsync();
        
        Task<MonitoredProduct> GetAsync(string id);

        Task<MonitoredProduct> GetByUrlAsync(string url);

        Task<MonitoredProduct> UpdateAsync(string id, MonitoredProduct data);

        Task<MonitoredProduct> InsertAsync(MonitoredProduct data);
    }
}