using System.Threading.Tasks;

namespace PriceAlerts.Api.Factories
{
    public interface IUserAlertFactory
    {
        Task<Api.Models.UserAlert> CreateUserAlert(Common.Models.UserAlert repoAlert);
    }
}