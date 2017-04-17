using System.Threading.Tasks;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IProductRepository
    {
        Task<MonitoredProduct> GetAsync(string id);

        Task<MonitoredProduct> GetByUrlAsync(string url);

        Task<bool> UpdateAsync(string id, MonitoredProduct data);

        Task<bool> InsertAsync(MonitoredProduct data);
    }
}