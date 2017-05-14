using System.Threading.Tasks;

using PriceAlerts.Api.Models;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Factories
{
    public interface IUserAlertFactory
    {
        Task<UserAlertDto> CreateUserAlert(UserAlert repoAlert);
    }
}