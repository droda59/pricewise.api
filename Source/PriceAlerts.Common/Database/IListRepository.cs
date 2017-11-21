using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IListRepository
    {
        [LoggingDescription("Create new user list in database")]
        Task<List> InsertAsync(List data);

        [LoggingDescription("Get user lists from database")]
        Task<IEnumerable<List>> GetUserListsAsync(string userId);
        
        [LoggingDescription("Get user list from database")]
        Task<List> GetAsync(string listId);

        [LoggingDescription("Remove user list from database")]
        Task<bool> DeleteAsync(string listId);
    }
}