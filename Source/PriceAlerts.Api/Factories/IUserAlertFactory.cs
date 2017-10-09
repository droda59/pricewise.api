using System.Threading.Tasks;

using PriceAlerts.Api.Models;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Factories
{
    public interface IUserAlertFactory
    {
        [LoggingDescription("Create user alert dto")]
        Task<UserAlertDto> CreateUserAlert(UserAlert repoAlert);

        [LoggingDescription("Create user alert summary dto")]
        Task<UserAlertSummaryDto> CreateUserAlertSummary(UserAlert repoAlert);
    }
}