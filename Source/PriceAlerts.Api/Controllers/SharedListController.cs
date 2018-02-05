using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class SharedListController : Controller
    {
        private readonly IListRepository _listRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserAlertFactory _userAlertFactory;
        private readonly IAlertListFactory _alertListFactory;

        public SharedListController(
            IListRepository listRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IUserAlertFactory userAlertFactory,
            IAlertListFactory alertListFactory,
            IProductFactory productFactory)
        {
            this._listRepository = listRepository;
            this._userRepository = userRepository;
            this._productRepository = productRepository;
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
            var sharedList = await this._alertListFactory.CreateAlertList<SharedListDto>(repoList, alert => alert.IsActive && !alert.IsDeleted);

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
            
            var alert = repoList.Alerts.FirstOrDefault(x => x.Id == alertId);
            if (alert == null || alert.IsDeleted)
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
            
            var alert = repoList.Alerts.FirstOrDefault(x => x.Id == alertId);
            if (alert == null || alert.IsDeleted)
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

        [HttpGet("{listId}/{alertId}/history")]
        [LoggingDescription("*** REQUEST to get shared list alert history ***")]
        public virtual async Task<IActionResult> GetSharedUserListAlertHistory(string listId, string alertId)
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
            
            var alert = repoList.Alerts.FirstOrDefault(x => x.Id == alertId);
            if (alert == null || alert.IsDeleted)
            {
                return this.NotFound();
            }
            
            if (!alert.IsActive)
            {
                return this.Unauthorized();
            }
            
            var lockObject = new object();
            var alertProducts = new List<MonitoredProduct>();
            await Task.WhenAll(alert.Entries.Where(x => !x.IsDeleted).Select(async entry => 
            {
                var existingProduct = await this._productRepository.GetAsync(entry.MonitoredProductId);

                lock (lockObject) 
                {
                    alertProducts.Add(existingProduct);
                }
            }));

            var deals = alertProducts
                .Select(x => new ProductHistory
                {
                    Title = x.Title,
                    Url = x.Uri,
                    PriceHistory = x.PriceHistory.GroupBy(y => y.ModifiedAt.Date).Select(y => y.Min())
                });

            return this.Ok(deals);
        }
    }
}