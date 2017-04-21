using System.Collections.Generic;
using System.Threading.Tasks;

using MongoDB.Driver;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    internal class UserRepository : EntityRepository<User>, IUserRepository
    {
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await this.Collection.Find(FilterDefinition<User>.Empty).ToListAsync();
        }

        public async Task<User> GetAsync(string userId)
        {
            return await this.Collection.Find(x => x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string userId, User data)
        {
            var result = await this.Collection.FindOneAndReplaceAsync(x => x.UserId == userId, data);

            return result != null;
        }

        public async Task<bool> InsertAsync(User data)
        {
            await this.Collection.InsertOneAsync(data);

            return true;
        }
    }
}