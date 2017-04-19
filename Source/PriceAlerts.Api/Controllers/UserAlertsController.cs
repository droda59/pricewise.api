using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using PriceAlerts.Common.Database;
using PriceAlerts.Common.Factories;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class UserAlertsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly PriceAlerts.Api.Factories.IUserAlertFactory _userAlertFactory;
        private readonly IProductFactory _productFactory;

        public UserAlertsController(IProductRepository productRepository, IUserRepository userRepository, PriceAlerts.Api.Factories.IUserAlertFactory userAlertFactory, IProductFactory productFactory)
        {
            this._productRepository = productRepository;
            this._userRepository = userRepository;
            this._userAlertFactory = userAlertFactory;
            this._productFactory = productFactory;
        }

        [HttpGet("{userId}/{alertId}")]
        public async Task<PriceAlerts.Api.Models.UserAlert> Get(string userId, string alertId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            var repoUserAlert = repoUser.Alerts.First(x => x.Id == alertId);

            var userAlert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

            return userAlert;
        }

        [HttpPost("{userId}")]
        public async Task<PriceAlerts.Api.Models.UserAlert> CreateAlert(string userId, [FromBody]Uri uri)
        {
            var existingProduct = await ForceGetProduct(uri.AbsoluteUri);
            var newAlert = new PriceAlerts.Common.Models.UserAlert 
            { 
                Id = ObjectId.GenerateNewId().ToString(), 
                Title = existingProduct.Title, 
                ImageUrl = existingProduct.ImageUrl,
                IsActive = true,
                BestCurrentDeal = existingProduct.PriceHistory.OrderBy(x => x.ModifiedAt).Last()
            };

            newAlert.Entries.Add(new PriceAlerts.Common.Models.UserAlertEntry { MonitoredProductId = existingProduct.Id });

            var repoUser = await this._userRepository.GetAsync(userId);
            repoUser.Alerts.Add(newAlert);

            await this._userRepository.UpdateAsync(userId, repoUser);

            var userAlert = await this._userAlertFactory.CreateUserAlert(newAlert);

            return userAlert;
        }

        [HttpPut("{userId}")]
        public async Task<PriceAlerts.Api.Models.UserAlert> UpdateAlert(string userId, [FromBody]PriceAlerts.Api.Models.UserAlert alert)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            
            var repoUserAlert = repoUser.Alerts.First(x => x.Id == alert.Id);
            repoUserAlert.IsActive = alert.IsActive;
            repoUserAlert.Title = alert.Title;
            repoUserAlert.Entries.Clear();

            var alertProducts = new List<Common.Models.MonitoredProduct>();
            await Task.WhenAll(alert.Entries.Select(async entry => 
            {
                var existingProduct = await ForceGetProduct(entry.Uri);

                if (!entry.IsDeleted) 
                {
                    alertProducts.Add(existingProduct);
                }

                repoUserAlert.Entries.Add(
                    new PriceAlerts.Common.Models.UserAlertEntry 
                    {
                        MonitoredProductId = existingProduct.Id,
                        IsDeleted = entry.IsDeleted
                    });
            }));

            foreach(var product in alertProducts)
            {
                var lastPrice = product.PriceHistory.OrderBy(y => y.ModifiedAt).Last();
                if (lastPrice.Price < repoUserAlert.BestCurrentDeal.Price)
                {
                    repoUserAlert.BestCurrentDeal = lastPrice;
                }
            }

            await this._userRepository.UpdateAsync(userId, repoUser);

            var userAlert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

            return userAlert;
        }

        [HttpDelete("{userId}/{alertId}")]
        public async Task<bool> DeleteAlert(string userId, string alertId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            
            var repoUserAlert = repoUser.Alerts.First(x => x.Id == alertId);
            repoUserAlert.IsDeleted = true;
            repoUserAlert.IsActive = false;

            return await this._userRepository.UpdateAsync(userId, repoUser);
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
