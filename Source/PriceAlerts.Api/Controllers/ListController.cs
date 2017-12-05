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
        private readonly IAlertListFactory _alertListFactory;

        public ListController(
            IAlertRepository alertRepository,
            IListRepository listRepository,
            IAlertListFactory alertListFactory, 
            IProductFactory productFactory)
        {
            this._alertRepository = alertRepository;
            this._listRepository = listRepository;
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
            
            if (repoList.UserId != userId)
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
            var repoLists = await this._listRepository.GetAlertListsAsync(userId);
            
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

        [HttpPut("{userId}")]
        [LoggingDescription("*** REQUEST to update an alert list ***")]
        public virtual async Task<IActionResult> Update(string userId, [FromBody]ListDto list)
        {
            var repoList = await this._listRepository.GetAsync(list.Id);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
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

        [HttpPost("{userId}/{listId}")]
        [LoggingDescription("*** REQUEST to share an alert list ***")]
        public virtual async Task<IActionResult> Share(string userId, string listId)
        {
            var repoList = await this._listRepository.GetAsync(listId);
            if (repoList == null)
            {
                return this.NotFound();
            }
            
            if (repoList.UserId != userId)
            {
                return this.Unauthorized();
            }

            if (!repoList.IsPublic)
            {
                repoList.IsPublic = true;
                
                await this._listRepository.UpdateAsync(repoList);
            }

            return this.Ok(new Uri($"/list/{repoList.Id}", UriKind.Relative));
        }
    }
}