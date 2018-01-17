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

        [LoggingDescription("Get user from database by id")]
        Task<User> GetAsync(string userId);

        [LoggingDescription("Update user in database")]
        Task<User> UpdateAsync(string userId, User data);

        [LoggingDescription("Create new user in database")]
        Task<User> InsertAsync(User data);

        [LoggingDescription("Get user from database by email")]
        Task<User> GetByEmailAsync(string email);
    }
}