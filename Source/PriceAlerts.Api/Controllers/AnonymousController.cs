using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AnonymousController : Controller
    {
        private IAlertRepository _alertRepository;
        private readonly IUserRepository _userRepository;

        public AnonymousController(IAlertRepository alertRepository, IUserRepository userRepository)
        {
            this._alertRepository = alertRepository;
            this._userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPut("{email}/{alertId}/DesactivateAlert")]
        [LoggingDescription("*** REQUEST to desactivate an alert from an email ***")]
        public async Task<IActionResult> DesactivateAlert(string email, string alertId)
        {
            try
            {
                var user = await this._userRepository.GetByEmailAsync(email);
                var alert = user.Alerts.FirstOrDefault(x => x.Id == alertId);

                alert.IsActive = false;
                await this._alertRepository.UpdateAsync(user.UserId, alert);

                return this.Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

    }
}