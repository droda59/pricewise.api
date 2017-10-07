using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IAlertRepository
    {
        [LoggingDescription("Get all alerts from user")]
        Task<IEnumerable<UserAlert>> GetAllAsync(string userId);
        
        [LoggingDescription("Get alert from user")]
        Task<UserAlert> GetAsync(string userId, string alertId);

        [LoggingDescription("Update user alert")]
        Task<UserAlert> UpdateAsync(string userId, UserAlert data);

        [LoggingDescription("Create new user alert")]
        Task<UserAlert> InsertAsync(string userId, UserAlert data);

        [LoggingDescription("Delete alert from user")]
        Task<bool> DeleteAsync(string userId, string alertId);
    }
}