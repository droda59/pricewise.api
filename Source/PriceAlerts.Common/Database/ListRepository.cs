using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    internal class ListRepository : EntityRepository<List>, IListRepository
    {
        public async Task<IEnumerable<List>> GetUserAlertListsAsync(string userId)
        {
            return await this.Collection.Find(x => !x.IsDeleted && x.UserId == userId).ToListAsync();
        }
        
        public async Task<IEnumerable<List>> GetUserWatchedAlertListsAsync(string userId)
        {
            return await this.Collection.Find(x => !x.IsDeleted && x.IsPublic && x.Watchers.Contains(userId)).ToListAsync();
        }
        
        public async Task<List> GetAsync(string listId)
        {
            return await this.Collection.Find(x => !x.IsDeleted && x.Id == listId).FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteAsync(string listId)
        {
            var list = await this.Collection.Find(x => x.Id == listId).FirstOrDefaultAsync();
            
            list.IsDeleted = true;
            list.LastModifiedAt = DateTime.UtcNow;

            await this.Collection.FindOneAndReplaceAsync<List>(
                x => x.Id == listId, 
                list,
                new FindOneAndReplaceOptions<List> { ReturnDocument = ReturnDocument.After });

            return true;
        }
        
        public async Task<List> UpdateAsync(List data)
        {
            data.LastModifiedAt = DateTime.UtcNow;
            
            var updatedEntry = await this.Collection.FindOneAndReplaceAsync<List>(
                x => x.Id == data.Id, 
                data,
                new FindOneAndReplaceOptions<List> { ReturnDocument = ReturnDocument.After });

            return updatedEntry;
        }
        
        public async Task<List> InsertAsync(List data)
        {
            data.CreatedAt = DateTime.UtcNow;
            data.LastModifiedAt = DateTime.UtcNow;
            
            await this.Collection.InsertOneAsync(data);

            var insertedEntry = await this.GetAsync(data.Id);

            return insertedEntry;
        }
    }
}