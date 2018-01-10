using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IListRepository
    {
        [LoggingDescription("Create new alert list in database")]
        Task<List> InsertAsync(List data);

        [LoggingDescription("Get alert lists from database")]
        Task<IEnumerable<List>> GetUserAlertListsAsync(string userId);

        [LoggingDescription("Get watched alert lists from database")]
        Task<IEnumerable<List>> GetUserWatchedAlertListsAsync(string userId);
        
        [LoggingDescription("Get alert list from database")]
        Task<List> GetAsync(string listId);

        [LoggingDescription("Update alert list in database")]
        Task<List> UpdateAsync(List data);

        [LoggingDescription("Remove alert list from database")]
        Task<bool> DeleteAsync(string listId);
    }
}