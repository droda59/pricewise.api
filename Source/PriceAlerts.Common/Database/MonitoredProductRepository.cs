using System.Threading.Tasks;

using MongoDB.Driver;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    internal class MonitoredProductRepository : EntityRepository<MonitoredProduct>, IProductRepository
    {
        public async Task<MonitoredProduct> GetAsync(string id)
        {
            return await this.Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<MonitoredProduct> GetByUrlAsync(string url)
        {
            return await this.Collection.Find(x => x.Uri == url).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, MonitoredProduct data)
        {
            var result = await this.Collection.FindOneAndReplaceAsync(x => x.Id == id, data);

            return result != null;
        }

        public async Task<bool> InsertAsync(MonitoredProduct data)
        {
            await this.Collection.InsertOneAsync(data);

            return true;
        }
    }
}