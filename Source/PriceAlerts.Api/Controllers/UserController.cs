using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            if (repoUser == null)
            {
                return this.NotFound();
            }

            var user = new Api.Models.User()
            {
                UserId = repoUser.UserId,
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

            return this.Ok(user);
        }

        [Authorize]
        [HttpPost("{userId}")]
        public async Task<IActionResult> Create([FromBody]Api.Models.User user)
        {
            var repoUser = new Common.Models.User()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            var newUser = await this._userRepository.InsertAsync(repoUser);
            if (newUser != null)
            {
                return this.Ok(newUser);
            }
            
            return this.BadRequest();
        }

        [Authorize]
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(string userId, [FromBody]Api.Models.User user)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            repoUser.FirstName = user.FirstName;
            repoUser.LastName = user.LastName;
            repoUser.Email = user.Email;

            var updatedUser = await this._userRepository.UpdateAsync(userId, repoUser);
            if (updatedUser != null)
            {
                return this.Ok(updatedUser);
            }
            
            return this.BadRequest();
        }

        [Authorize]
        [HttpPut("{userId}/settings")]
        public async Task<IActionResult> UpdateSettings(string userId, [FromBody]Common.Models.Settings userSettings)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            repoUser.Settings.AlertOnPriceDrop = userSettings.AlertOnPriceDrop;
            repoUser.Settings.AlertOnPriceRaise = userSettings.AlertOnPriceRaise;
            repoUser.Settings.SpecifyChangePercentage = userSettings.SpecifyChangePercentage;
            repoUser.Settings.ChangePercentage = userSettings.ChangePercentage;

            var updatedUser = await this._userRepository.UpdateAsync(userId, repoUser);
            if (updatedUser != null)
            {
                return this.Ok(updatedUser);
            }
            
            return this.BadRequest();
        }
    }
}
