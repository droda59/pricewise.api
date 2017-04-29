using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
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
        private readonly Api.Factories.IUserAlertFactory _userAlertFactory;
        private readonly IProductFactory _productFactory;

        public UserAlertsController(IProductRepository productRepository, IUserRepository userRepository, Api.Factories.IUserAlertFactory userAlertFactory, IProductFactory productFactory)
        {
            this._productRepository = productRepository;
            this._userRepository = userRepository;
            this._userAlertFactory = userAlertFactory;
            this._productFactory = productFactory;
        }

        [Authorize]
        [HttpGet("{userId}/{alertId}")]
        public async Task<Api.Models.UserAlert> Get(string userId, string alertId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            var repoUserAlert = repoUser.Alerts.Single(x => x.Id == alertId);

            var userAlert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

            return userAlert;
        }

        [Authorize]
        [HttpGet("{userId}/{alertId}/history")]
        public async Task<IEnumerable<Api.Models.ProductHistory>> GetHistory(string userId, string alertId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            var repoUserAlert = repoUser.Alerts.Single(x => x.Id == alertId);

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
                    PriceHistory = x.PriceHistory.GroupBy(y => y.ModifiedAt.Date).Select(y => y.Min())
                });

            return deals;
        }

        [Authorize]
        [HttpPost("{userId}")]
        public async Task<Api.Models.UserAlert> CreateAlert(string userId, [FromBody]Uri uri)
        {
            var existingProduct = await ForceGetProduct(uri.AbsoluteUri);
            var newAlert = new Common.Models.UserAlert 
            { 
                Id = ObjectId.GenerateNewId().ToString(), 
                Title = existingProduct.Title, 
                ImageUrl = existingProduct.ImageUrl,
                IsActive = true,
                BestCurrentDeal = existingProduct.PriceHistory.OrderBy(x => x.ModifiedAt).Last()
            };

            newAlert.Entries.Add(new Common.Models.UserAlertEntry { MonitoredProductId = existingProduct.Id });

            var repoUser = await this._userRepository.GetAsync(userId);
            repoUser.Alerts.Add(newAlert);

            await this._userRepository.UpdateAsync(userId, repoUser);

            var userAlert = await this._userAlertFactory.CreateUserAlert(newAlert);

            return userAlert;
        }

        [Authorize]
        [HttpPut("{userId}")]
        public async Task<Api.Models.UserAlert> UpdateAlert(string userId, [FromBody]Api.Models.UserAlert alert)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            
            var repoUserAlert = repoUser.Alerts.Single(x => x.Id == alert.Id);
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
                    new Common.Models.UserAlertEntry 
                    {
                        MonitoredProductId = existingProduct.Id,
                        IsDeleted = entry.IsDeleted
                    });
            }));

            var bestDeal = alertProducts
                .Select(x => x.PriceHistory.OrderBy(y => y.ModifiedAt).Last())
                .Select(x => Tuple.Create(x.Price, x))
                .OrderBy(x => x.Item1)
                .First();

            repoUserAlert.BestCurrentDeal = bestDeal.Item2;

            await this._userRepository.UpdateAsync(userId, repoUser);

            var userAlert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

            return userAlert;
        }

        [Authorize]
        [HttpDelete("{userId}/{alertId}")]
        public async Task<bool> DeleteAlert(string userId, string alertId)
        {
            var repoUser = await this._userRepository.GetAsync(userId);
            
            var repoUserAlert = repoUser.Alerts.Single(x => x.Id == alertId);
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
