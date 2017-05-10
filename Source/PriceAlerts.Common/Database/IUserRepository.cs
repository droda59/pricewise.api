using System.Collections.Generic;
using System.Threading.Tasks;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        
        Task<User> GetAsync(string userId);

        Task<User> UpdateAsync(string userId, User data);

        Task<User> InsertAsync(User data);
    }
}