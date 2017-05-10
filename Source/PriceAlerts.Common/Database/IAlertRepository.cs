using System.Threading.Tasks;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IAlertRepository
    {
        Task<UserAlert> GetAsync(string userId, string alertId);

        Task<UserAlert> UpdateAsync(string userId, UserAlert data);

        Task<UserAlert> InsertAsync(string userId, UserAlert data);

        Task<bool> DeleteAsync(string userId, string alertId);
    }
}