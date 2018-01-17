using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class EmailNotificationController : Controller
    {
        private IAlertRepository _alertRepository;
        private readonly IUserRepository _userRepository;

        public EmailNotificationController(IAlertRepository alertRepository, IUserRepository userRepository)
        {
            this._alertRepository = alertRepository;
            this._userRepository = userRepository;
        }

        [HttpPut("{email}/{alertId}/deactivate")]
        [LoggingDescription("*** REQUEST to deactivate an alert from an email ***")]
        public async Task<IActionResult> DeactivateAlert(string email, string alertId)
        {
            var user = await this._userRepository.GetByEmailAsync(email);
            var alert = user?.Alerts.FirstOrDefault(x => x.Id == alertId);

            if (user == null || alert == null)
            {
                return this.NotFound("User or alert not found");
            }

            alert.IsActive = false;
            await this._alertRepository.UpdateAsync(user.UserId, alert);

            return this.Ok();
        }
    }
}