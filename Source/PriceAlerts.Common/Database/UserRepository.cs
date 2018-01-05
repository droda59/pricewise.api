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

        public async Task<User> GetByEmailAsync(string email)
        {
            var emailTofind = email.ToLower();
            return await this.Collection.Find(x => x.Email.ToLower() == emailTofind).FirstOrDefaultAsync();
        }

        public async Task<User> UpdateAsync(string userId, User data)
        {
            var updatedEntry = await this.Collection.FindOneAndReplaceAsync<User>(
                x => x.UserId == userId,
                data,
                new FindOneAndReplaceOptions<User> { ReturnDocument = ReturnDocument.After });

            return updatedEntry;
        }

        public async Task<User> InsertAsync(User data)
        {
            await this.Collection.InsertOneAsync(data);

            var insertedEntry = await this.GetAsync(data.UserId);

            return insertedEntry;
        }
    }
}