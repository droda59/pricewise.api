using System.Threading.Tasks;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Factories
{
    public interface IProductFactory
    {
        Task<MonitoredProduct> CreateProduct(string uri);
    }
}