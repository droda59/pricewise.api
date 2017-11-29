using System.Collections.Generic;
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
        private readonly IUserAlertFactory _userAlertFactory;

        public SharedListController(
            IListRepository listRepository,
            IUserAlertFactory userAlertFactory, 
            IProductFactory productFactory)
        {
            this._listRepository = listRepository;
            this._userAlertFactory = userAlertFactory;
        }
        
        [HttpGet("{listId}")]
        [LoggingDescription("*** REQUEST to get shared user list ***")]
        public virtual async Task<IActionResult> GetSharedUserList(string listId)
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
            
            var lockObject = new object();
            var summaries = new List<UserAlertSummaryDto>();
            await Task.WhenAll(repoList.Alerts.Select(async alert =>
            {
                var summary = await this._userAlertFactory.CreateUserAlertSummary(alert);
                lock (lockObject) 
                {
                    summaries.Add(summary);
                }
            }));

            var userList = new ListDto
            {
                Id = repoList.Id,
                Name = repoList.Name,
                Alerts = summaries
            };

            return this.Ok(userList);
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