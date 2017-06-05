using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserAlertFactory _userAlertFactory;

        public UserController(IUserRepository userRepository, IUserAlertFactory userAlertFactory)
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

            var userDto = CreateDto(repoUser);

            var lockObject = new Object();
            var notDeletedAlerts = repoUser.Alerts.Where(x => !x.IsDeleted).ToList();
            await Task.WhenAll(notDeletedAlerts.Select(async repoUserAlert => 
            {
                var alert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

                lock(lockObject)
                {
                    userDto.Alerts.Add(alert);
                }  
            }));

            return this.Ok(userDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]UserDto user)
        {
            var repoUser = new User()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            var newUser = await this._userRepository.InsertAsync(repoUser);
            if (newUser != null)
            {
                return this.Ok(CreateDto(newUser));
            }
            
            return this.BadRequest();
        }

        [Authorize]
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(string userId, [FromBody]UserDto user)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            repoUser.FirstName = user.FirstName;
            repoUser.LastName = user.LastName;
            repoUser.Email = user.Email;

            var updatedUser = await this._userRepository.UpdateAsync(userId, repoUser);
            if (updatedUser != null)
            {
                return this.Ok(CreateDto(updatedUser));
            }

            return this.BadRequest();
        }

        [Authorize]
        [HttpPut("{userId}/settings")]
        public async Task<IActionResult> UpdateSettings(string userId, [FromBody]Settings userSettings)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            repoUser.Settings.AlertOnPriceDrop = userSettings.AlertOnPriceDrop;
            repoUser.Settings.AlertOnPriceRaise = userSettings.AlertOnPriceRaise;
            repoUser.Settings.SpecifyChangePercentage = userSettings.SpecifyChangePercentage;
            repoUser.Settings.ChangePercentage = userSettings.ChangePercentage;

            var updatedUser = await this._userRepository.UpdateAsync(userId, repoUser);
            if (updatedUser != null)
            {
                return this.Ok(updatedUser.Settings);
            }
            
            return this.BadRequest();
        }

        private static UserDto CreateDto(User repoUser)
        {
            return new UserDto()
            {
                UserId = repoUser.UserId,
                FirstName = repoUser.FirstName,
                LastName = repoUser.LastName,
                Email = repoUser.Email,
                Settings = repoUser.Settings
            };
        }
    }
}
