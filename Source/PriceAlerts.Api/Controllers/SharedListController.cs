using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PriceAlerts.Api.Factories;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class SharedListController : Controller
    {
        private readonly IListRepository _listRepository;
        private readonly IUserAlertFactory _userAlertFactory;
        private readonly IAlertListFactory _alertListFactory;

        public SharedListController(
            IListRepository listRepository,
            IUserAlertFactory userAlertFactory,
            IAlertListFactory alertListFactory,
            IProductFactory productFactory)
        {
            this._listRepository = listRepository;
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

            var alertList = await this._alertListFactory.CreateAlertList(repoList);

            return this.Ok(alertList);
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
            var summary = await this._userAlertFactory.CreateUserAlert(alert);

            return this.Ok(summary);
        }
    }
}