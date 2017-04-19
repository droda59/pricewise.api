using System.Collections.Generic;
using System.Threading.Tasks;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        
        Task<User> GetAsync(string id);

        Task<bool> UpdateAsync(string id, User data);

        Task<bool> InsertAsync(User data);
    }
}