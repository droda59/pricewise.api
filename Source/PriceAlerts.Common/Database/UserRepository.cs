using System.Threading.Tasks;

using MongoDB.Driver;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    internal class UserRepository : EntityRepository<User>, IUserRepository
    {
        public async Task<User> GetAsync(string id)
        {
            return await this.Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(string id, User data)
        {
            var result = await this.Collection.FindOneAndReplaceAsync(x => x.Id == id, data);

            return result != null;
        }

        public async Task<bool> InsertAsync(User data)
        {
            await this.Collection.InsertOneAsync(data);

            return true;
        }
    }
}