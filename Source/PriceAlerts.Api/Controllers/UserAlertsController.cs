using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Common;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Factories;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserAlertsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly Api.Factories.IUserAlertFactory _userAlertFactory;
        private readonly IProductFactory _productFactory;

        public UserAlertsController(IProductRepository productRepository, IAlertRepository alertRepository, Api.Factories.IUserAlertFactory userAlertFactory, IProductFactory productFactory)
        {
            this._productRepository = productRepository;
            this._alertRepository = alertRepository;
            this._userAlertFactory = userAlertFactory;
            this._productFactory = productFactory;
        }

        [Authorize]
        [HttpGet("{userId}/{alertId}")]
        public async Task<Api.Models.UserAlert> Get(string userId, string alertId)
        {
            var repoUserAlert = await this._alertRepository.GetAsync(userId, alertId);

            var userAlert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

            return userAlert;
        }

        [Authorize]
        [HttpGet("{userId}/{alertId}/history")]
        public async Task<IEnumerable<Api.Models.ProductHistory>> GetHistory(string userId, string alertId)
        {
            var repoUserAlert = await this._alertRepository.GetAsync(userId, alertId);

            var alertProducts = new List<Common.Models.MonitoredProduct>();
            await Task.WhenAll(repoUserAlert.Entries.Where(x => !x.IsDeleted).Select(async entry => 
            {
                var existingProduct = await this._productRepository.GetAsync(entry.MonitoredProductId);
                alertProducts.Add(existingProduct);
            }));

            var deals = alertProducts
                .Select(x => new Models.ProductHistory
                {
                    Title = x.Title,
                    Url = x.Uri,
                    PriceHistory = x.PriceHistory.GroupBy(y => y.ModifiedAt.Date).Select(y => y.Min())
                });

            return deals;
        }

        [Authorize]
        [HttpPost("{userId}")]
        public async Task<Api.Models.UserAlert> CreateAlert(string userId, [FromBody]Uri uri)
        {
            var existingProduct = await ForceGetProduct(uri.AbsoluteUri);
            var currentPrice = existingProduct.PriceHistory.LastOf(x => x.ModifiedAt);
            var newAlert = new Common.Models.UserAlert 
            { 
                Title = existingProduct.Title, 
                ImageUrl = existingProduct.ImageUrl,
                IsActive = true,
                BestCurrentDeal = new Common.Models.Deal { Price = currentPrice.Price, ModifiedAt = currentPrice.ModifiedAt, ProductId = existingProduct.Id }
            };

            newAlert.Entries.Add(new Common.Models.UserAlertEntry { MonitoredProductId = existingProduct.Id });

            newAlert = await this._alertRepository.InsertAsync(userId, newAlert);

            var userAlert = await this._userAlertFactory.CreateUserAlert(newAlert);

            return userAlert;
        }

        [Authorize]
        [HttpPut("{userId}")]
        public async Task<Api.Models.UserAlert> UpdateAlert(string userId, [FromBody]Api.Models.UserAlert alert)
        {
            var repoUserAlert = await this._alertRepository.GetAsync(userId, alert.Id);
            repoUserAlert.IsActive = alert.IsActive;
            repoUserAlert.Title = alert.Title;
            repoUserAlert.Entries.Clear();

            var alertProducts = new List<Common.Models.MonitoredProduct>();
            foreach (var entry in alert.Entries)
            {
                var existingProduct = await ForceGetProduct(entry.Uri);

                if (!entry.IsDeleted) 
                {
                    alertProducts.Add(existingProduct);
                }

                repoUserAlert.Entries.Add(
                    new Common.Models.UserAlertEntry 
                    {
                        MonitoredProductId = existingProduct.Id,
                        IsDeleted = entry.IsDeleted
                    });
            }

            var bestDeal = alertProducts
                .Select(p => Tuple.Create(p, p.PriceHistory.LastOf(y => y.ModifiedAt)))
                .FirstOf(x => x.Item2.Price);

            repoUserAlert.BestCurrentDeal = new Common.Models.Deal { ProductId = bestDeal.Item1.Id, Price = bestDeal.Item2.Price, ModifiedAt = bestDeal.Item2.ModifiedAt };

            repoUserAlert = await this._alertRepository.UpdateAsync(userId, repoUserAlert);

            var userAlert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

            return userAlert;
        }

        [Authorize]
        [HttpDelete("{userId}/{alertId}")]
        public async Task<bool> DeleteAlert(string userId, string alertId)
        {
            var isDeleted = await this._alertRepository.DeleteAsync(userId, alertId);

            return isDeleted;
        }

        private async Task<Common.Models.MonitoredProduct> ForceGetProduct(string uri)
        {
            var existingProduct = await this._productRepository.GetByUrlAsync(uri);
            if (existingProduct == null)
            {
                existingProduct = await this._productFactory.CreateProduct(uri);
            }

            return existingProduct;
        }
    }
}
