using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class SharedListController : Controller
    {
        private readonly IListRepository _listRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserAlertFactory _userAlertFactory;
        private readonly IAlertListFactory _alertListFactory;

        public SharedListController(
            IListRepository listRepository,
            IUserRepository userRepository,
            IUserAlertFactory userAlertFactory,
            IAlertListFactory alertListFactory,
            IProductFactory productFactory)
        {
            this._listRepository = listRepository;
            this._userRepository = userRepository;
            this._userAlertFactory = userAlertFactory;
            this._alertListFactory = alertListFactory;
        }
        
        [HttpGet("{listId}")]
        [LoggingDescription("*** REQUEST to get shared user list ***")]
        public virtual async Task<IActionResult> GetSharedAlertList(string listId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
            if (!repoList.IsPublic)
            {
                return this.Unauthorized();
            }

            var user = await this._userRepository.GetAsync(repoList.UserId);
            var sharedList = await this._alertListFactory.CreateAlertList<SharedListDto>(repoList, alert => alert.IsActive);

            sharedList.UserName = $"{user.FirstName}";

            return this.Ok(sharedList);
        }

        [HttpGet("{listId}/{alertId}/summary")]
        [LoggingDescription("*** REQUEST to get shared list alert summary ***")]
        public virtual async Task<IActionResult> GetSharedUserListAlertSummary(string listId, string alertId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
            if (!repoList.IsPublic)
            {
                return this.Unauthorized();
            }
            
            var alert = repoList.Alerts.SingleOrDefault(x => x.Id == alertId);
            
            if (alert == null || !alert.IsDeleted)
            {
                return this.NotFound();
            }
            
            if (!alert.IsActive)
            {
                return this.Unauthorized();
            }
            
            var summary = await this._userAlertFactory.CreateUserAlertSummary(alert);

            return this.Ok(summary);
        }

        [HttpGet("{listId}/{alertId}")]
        [LoggingDescription("*** REQUEST to get shared list alert details ***")]
        public virtual async Task<IActionResult> GetSharedUserListAlertDetails(string listId, string alertId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
            if (!repoList.IsPublic)
            {
                return this.Unauthorized();
            }
            
            var alert = repoList.Alerts.SingleOrDefault(x => x.Id == alertId);
            
            if (alert == null || !alert.IsDeleted)
            {
                return this.NotFound();
            }
            
            if (!alert.IsActive)
            {
                return this.Unauthorized();
            }
            
            var details = await this._userAlertFactory.CreateUserAlert(alert);

            return this.Ok(details);
        }
    }
}