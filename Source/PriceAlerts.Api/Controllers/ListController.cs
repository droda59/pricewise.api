using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ListController : Controller
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IListRepository _listRepository;
        private readonly IUserAlertFactory _userAlertFactory;

        public ListController(
            IAlertRepository alertRepository,
            IListRepository listRepository,
            IUserAlertFactory userAlertFactory, 
            IProductFactory productFactory)
        {
            this._alertRepository = alertRepository;
            this._listRepository = listRepository;
            this._userAlertFactory = userAlertFactory;
        }

        [HttpDelete("{userId}/{listId}")]
        [LoggingDescription("*** REQUEST to delete user list ***")]
        public virtual async Task<IActionResult> DeleteUserList(string userId, string listId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList.UserId != userId)
            {
                return this.Unauthorized();
            }

            if (repoList.IsDeleted)
            {
                return this.NotFound();
            }

            var result = await this._listRepository.DeleteAsync(listId);

            return this.Ok(result);
        }

        [HttpGet("{userId}/{listId}")]
        [LoggingDescription("*** REQUEST to get user list ***")]
        public virtual async Task<IActionResult> GetUserList(string userId, string listId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList.UserId != userId)
            {
                return this.Unauthorized();
            }

            if (repoList.IsDeleted)
            {
                return this.NotFound();
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

        [HttpGet("{userId}")]
        [LoggingDescription("*** REQUEST to get user lists ***")]
        public virtual async Task<IEnumerable<ListSummaryDto>> GetUserListSummaries(string userId)
        {
            var repoLists = await this._listRepository.GetUserListsAsync(userId);
            
            var userLists = new List<ListSummaryDto>();
            foreach (var repoList in repoLists)
            {
                userLists.Add(new ListSummaryDto
                {
                    Id = repoList.Id,
                    Name = repoList.Name
                });
            }

            return userLists;
        }

        [HttpPut("{userId}")]
        [LoggingDescription("*** REQUEST to update a list ***")]
        public virtual async Task<ListSummaryDto> UpdateList(string userId, [FromBody]ListDto list)
        {
            var repoList = await this._listRepository.GetAsync(list.Id);
            
            var repoAlerts = await this._alertRepository.GetAllAsync(userId);
            var listAlertIds = list.Alerts.Select(x => x.Id).ToList();
            var listAlerts = repoAlerts.Where(x => listAlertIds.Contains(x.Id)).ToList();

            repoList.Name = list.Name;
            repoList.Alerts = listAlerts;

            repoList = await this._listRepository.UpdateAsync(repoList);
            
            var userList = new ListSummaryDto
            {
                Id = repoList.Id,
                Name = repoList.Name
            };
            
            return userList;
        }

        [HttpPost("{userId}")]
        [LoggingDescription("*** REQUEST to create a list ***")]
        public virtual async Task<ListSummaryDto> CreateList(string userId, [FromBody]ListDto list)
        {
            var repoAlerts = await this._alertRepository.GetAllAsync(userId);
            var listAlertIds = list.Alerts.Select(x => x.Id).ToList();
            var listAlerts = repoAlerts.Where(x => listAlertIds.Contains(x.Id)).ToList();
            
            var newList = new List
            {
                Name = list.Name,
                UserId = userId,
                Alerts = listAlerts
            };

            newList = await this._listRepository.InsertAsync(newList);

            var userList = new ListSummaryDto
            {
                Id = newList.Id,
                Name = newList.Name
            };
            
            return userList;
        }
    }
}