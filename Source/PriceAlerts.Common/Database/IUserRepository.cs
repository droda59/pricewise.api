using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IUserRepository
    {
        [LoggingDescription("Get all users from database")]
        Task<IEnumerable<User>> GetAllAsync();
        
        [LoggingDescription("Get user from database")]
        Task<User> GetAsync(string userId);

        [LoggingDescription("Update database user")]
        Task<User> UpdateAsync(string userId, User data);

        [LoggingDescription("Create new database user")]
        Task<User> InsertAsync(User data);
    }
}