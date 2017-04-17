using System.Threading.Tasks;

using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IUserRepository
    {
        Task<User> GetAsync(string id);

        Task<bool> UpdateAsync(string id, User data);

        Task<bool> InsertAsync(User data);
    }
}