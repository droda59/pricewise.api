using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Api.Models;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
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

            return this.Ok(userDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]UserDto user)
        {
            var repoUser = new User
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
            repoUser.Settings.AlertOnPriceDrop = user.Settings.AlertOnPriceDrop;
            repoUser.Settings.AlertOnPriceRaise = user.Settings.AlertOnPriceRaise;
            repoUser.Settings.SpecifyChangePercentage = user.Settings.SpecifyChangePercentage;
            repoUser.Settings.ChangePercentage = user.Settings.ChangePercentage;
            repoUser.Settings.CorrespondenceLanguage = user.Settings.CorrespondenceLanguage;

            var updatedUser = await this._userRepository.UpdateAsync(userId, repoUser);
            if (updatedUser != null)
            {
                return this.Ok(CreateDto(updatedUser));
            }

            return this.BadRequest();
        }

        private static UserDto CreateDto(User repoUser)
        {
            return new UserDto
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
