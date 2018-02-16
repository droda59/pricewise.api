using System;
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
        private readonly IUserRepository _userRepository;
        private readonly IAlertListFactory _alertListFactory;

        public ListController(
            IAlertRepository alertRepository,
            IListRepository listRepository,
            IUserRepository userRepository,
            IAlertListFactory alertListFactory, 
            IProductFactory productFactory)
        {
            this._alertRepository = alertRepository;
            this._listRepository = listRepository;
            this._userRepository = userRepository;
            this._alertListFactory = alertListFactory;
        }

        [HttpDelete("{userId}/{listId}")]
        [LoggingDescription("*** REQUEST to delete alert list ***")]
        public virtual async Task<IActionResult> Delete(string userId, string listId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
            // Only the list's user can delete it
            if (repoList.UserId != userId)
            {
                return this.Unauthorized();
            }

            var result = await this._listRepository.DeleteAsync(listId);

            return this.Ok(result);
        }

        [HttpGet("{userId}/{listId}")]
        [LoggingDescription("*** REQUEST to get alert list ***")]
        public virtual async Task<IActionResult> GetAlertList(string userId, string listId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
            // You can get the list if you are a watcher
            if (repoList.UserId != userId && !repoList.Watchers.Contains(userId))
            {
                return this.Unauthorized();
            }
            
            var alertList = await this._alertListFactory.CreateAlertList(repoList);

            return this.Ok(alertList);
        }

        [HttpGet("{userId}")]
        [LoggingDescription("*** REQUEST to get alert lists ***")]
        public virtual async Task<IEnumerable<ListDto>> GetAlertLists(string userId)
        {
            var repoLists = await this._listRepository.GetUserAlertListsAsync(userId);
            
            var lockObject = new object();
            var alertLists = new List<ListDto>();
            await Task.WhenAll(repoLists.Select(async repoList =>
            {
                var alertList = await this._alertListFactory.CreateAlertList(repoList);
                lock (lockObject)
                {
                    alertLists.Add(alertList);
                }
            }));

            return alertLists;
        }

        [HttpGet("{userId}/watched")]
        [LoggingDescription("*** REQUEST to get watched alert lists ***")]
        public virtual async Task<IEnumerable<ListDto>> GetWatchedAlertLists(string userId)
        {
            var repoLists = await this._listRepository.GetUserWatchedAlertListsAsync(userId);
            
            var lockObject = new object();
            var alertLists = new List<ListDto>();
            await Task.WhenAll(repoLists.Select(async repoList =>
            {
                var alertList = await this._alertListFactory.CreateAlertList<ListDto>(repoList, alert => alert.IsActive && !alert.IsDeleted);
                lock (lockObject)
                {
                    alertLists.Add(alertList);
                }
            }));

            return alertLists;
        }

        [HttpPut("{userId}")]
        [LoggingDescription("*** REQUEST to update an alert list ***")]
        public virtual async Task<IActionResult> Update(string userId, [FromBody]ListDto list)
        {
            var repoList = await this._listRepository.GetAsync(list.Id);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
            // Only the list's user can update it
            if (repoList.UserId != userId)
            {
                return this.Unauthorized();
            }
            
            var repoAlerts = await this._alertRepository.GetAllAsync(userId);
            var listAlertIds = list.Alerts.Select(x => x.Id).ToList();
            var listAlerts = repoAlerts.Where(x => listAlertIds.Contains(x.Id)).ToList();

            repoList.Name = list.Name;
            repoList.Alerts = listAlerts;

            repoList = await this._listRepository.UpdateAsync(repoList);
            
            var alertList = await this._alertListFactory.CreateAlertList(repoList);
            
            return this.Ok(alertList);
        }

        [HttpPost("{userId}")]
        [LoggingDescription("*** REQUEST to create an alert list ***")]
        public virtual async Task<ListDto> Create(string userId, [FromBody]ListDto list)
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

            return await this._alertListFactory.CreateAlertList(newList);
        }

        [HttpPost("{userId}/{listId}/share")]
        [LoggingDescription("*** REQUEST to share an alert list ***")]
        public virtual async Task<IActionResult> Share(string userId, string listId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
            // Only the list's user can share it
            if (repoList.UserId != userId)
            {
                return this.Unauthorized();
            }

            if (!repoList.IsPublic)
            {
                repoList.IsPublic = true;
                
                await this._listRepository.UpdateAsync(repoList);
            }

            return this.Ok(new Uri($"list/{repoList.Id}", UriKind.Relative));
        }

        [HttpPost("{userId}/{listId}/unshare")]
        [LoggingDescription("*** REQUEST to share an alert list ***")]
        public virtual async Task<IActionResult> Unshare(string userId, string listId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
            // Only the list's user can unshare it
            if (repoList.UserId != userId)
            {
                return this.Unauthorized();
            }

            if (repoList.IsPublic)
            {
                repoList.IsPublic = false;
                
                await this._listRepository.UpdateAsync(repoList);
            }

            return this.Ok();
        }

        [HttpPost("{userId}/{listId}/follow")]
        [LoggingDescription("*** REQUEST to follow a public alert list ***")]
        public virtual async Task<IActionResult> FollowAlertList(string userId, string listId)
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
            
            // A user cannot follow its own list
            if (repoList.UserId == userId)
            {
                return this.BadRequest();
            }

            // A user cannot follow a list he is already following
            if (repoList.Watchers.Contains(userId))
            {
                return this.Ok();
            }
            
            repoList.Watchers.Add(userId);
            
            await this._listRepository.UpdateAsync(repoList);
            
            return this.Ok();
        }

        [HttpPost("{userId}/{listId}/unfollow")]
        [LoggingDescription("*** REQUEST to follow a public alert list ***")]
        public virtual async Task<IActionResult> UnfollowAlertList(string userId, string listId)
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
            
            // A user cannot unfollow its own list
            if (repoList.UserId == userId)
            {
                return this.BadRequest();
            }

            // A user cannot unfollow a list he is not already following
            if (!repoList.Watchers.Contains(userId))
            {
                return this.Ok();
            }
            
            repoList.Watchers.Remove(userId);
            
            await this._listRepository.UpdateAsync(repoList);
            
            return this.Ok();
        }
    }
}