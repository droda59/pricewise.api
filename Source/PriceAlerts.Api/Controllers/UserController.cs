using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Common.Database;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly PriceAlerts.Api.Factories.IUserAlertFactory _userAlertFactory;

        public UserController(IUserRepository userRepository, PriceAlerts.Api.Factories.IUserAlertFactory userAlertFactory)
        {
            this._userRepository = userRepository;
            this._userAlertFactory = userAlertFactory;
        }

        [HttpGet("{userId}")]
        public async Task<PriceAlerts.Api.Models.User> Get(string userId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);

            var user = new PriceAlerts.Api.Models.User()
            {
                Id = repoUser.Id,
                FirstName = repoUser.FirstName,
                LastName = repoUser.LastName,
                Email = repoUser.Email
            };

            var notDeletedAlerts = repoUser.Alerts.Where(x => !x.IsDeleted).ToList();
            await Task.WhenAll(notDeletedAlerts.Select(async repoUserAlert => 
            {
                var alert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

                user.Alerts.Add(alert);
            }));

            return user;
        }

        [HttpPut("{userId}")]
        public async Task<bool> Update(string userId, [FromBody]PriceAlerts.Api.Models.User user)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            repoUser.FirstName = user.FirstName;
            repoUser.LastName = user.LastName;
            repoUser.Email = user.Email;

            return await this._userRepository.UpdateAsync(userId, repoUser);
        }
    }
}
