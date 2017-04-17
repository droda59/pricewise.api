using System.Threading.Tasks;

namespace PriceAlerts.Api.Factories
{
    public interface IUserAlertFactory
    {
        Task<PriceAlerts.Api.Models.UserAlert> CreateUserAlert(PriceAlerts.Common.Models.UserAlert repoAlert);
    }
}