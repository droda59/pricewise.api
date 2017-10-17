using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Driver;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    internal class MonitoredProductRepository : EntityRepository<MonitoredProduct>, IProductRepository
    {
        public async Task<IEnumerable<MonitoredProduct>> GetAllAsync()
        {
            return await this.Collection.Find(FilterDefinition<MonitoredProduct>.Empty).ToListAsync();
        }

        public async Task<IEnumerable<MonitoredProduct>> GetAllByProductIdentifierAsync(string productIdentifier)
        {
            return await this.Collection.Find(x => x.ProductIdentifier == productIdentifier).ToListAsync();
        }

        public async Task<MonitoredProduct> GetAsync(string id)
        {
            return await this.Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<MonitoredProduct> GetByUrlAsync(string url)
        {
            return await this.Collection.Find(x => x.Uri == url).FirstOrDefaultAsync();
        }

        public async Task<MonitoredProduct> UpdateAsync(string id, MonitoredProduct data)
        {
            var updatedEntry = await this.Collection.FindOneAndReplaceAsync<MonitoredProduct>(
                x => x.Id == id, 
                data,
                new FindOneAndReplaceOptions<MonitoredProduct> { ReturnDocument = ReturnDocument.After });

            return updatedEntry;
        }

        public async Task<MonitoredProduct> InsertAsync(MonitoredProduct data)
        {
            await this.Collection.InsertOneAsync(data);

            var insertedEntry = await this.GetByUrlAsync(data.Uri);

            return insertedEntry;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await this.Collection.DeleteOneAsync(x => x.Id == id);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}