using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserAlertsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly IUserAlertFactory _userAlertFactory;
        private readonly IProductFactory _productFactory;
        private readonly IHandlerFactory _handlerFactory;

        public UserAlertsController(IProductRepository productRepository, IAlertRepository alertRepository, IUserAlertFactory userAlertFactory, IProductFactory productFactory, IHandlerFactory handlerFactory)
        {
            this._productRepository = productRepository;
            this._alertRepository = alertRepository;
            this._userAlertFactory = userAlertFactory;
            this._productFactory = productFactory;
            this._handlerFactory = handlerFactory;
        }

        [HttpGet("{userId}/{alertId}")]
        public async Task<UserAlertDto> Get(string userId, string alertId)
        {
            var repoUserAlert = await this._alertRepository.GetAsync(userId, alertId);

            var userAlert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

            return userAlert;
        }

        [HttpGet("{userId}/{alertId}/history")]
        public async Task<IEnumerable<ProductHistory>> GetHistory(string userId, string alertId)
        {
            var repoUserAlert = await this._alertRepository.GetAsync(userId, alertId);

            var lockObject = new Object();
            var alertProducts = new List<MonitoredProduct>();
            await Task.WhenAll(repoUserAlert.Entries.Where(x => !x.IsDeleted).Select(async entry => 
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

            return deals;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateAlert(string userId, [FromBody]Uri uri)
        {
            try 
            {
                var existingProduct = await ForceGetProduct(uri);
                var currentPrice = existingProduct.PriceHistory.LastOf(x => x.ModifiedAt);
                var newAlert = new UserAlert 
                { 
                    Title = existingProduct.Title, 
                    ImageUrl = existingProduct.ImageUrl,
                    IsActive = true,
                    BestCurrentDeal = new Deal { Price = currentPrice.Price, ModifiedAt = currentPrice.ModifiedAt, ProductId = existingProduct.Id }
                };

                newAlert.Entries.Add(new UserAlertEntry { MonitoredProductId = existingProduct.Id });

                newAlert = await this._alertRepository.InsertAsync(userId, newAlert);

                var userAlert = await this._userAlertFactory.CreateUserAlert(newAlert);

                return this.Ok(userAlert);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFound("The specified source is not yet supported.");
            }
            catch (ParseException e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAlert(string userId, [FromBody]UserAlertDto alert)
        {
            try
            {
                var repoUserAlert = await this._alertRepository.GetAsync(userId, alert.Id);
                repoUserAlert.IsActive = alert.IsActive;
                repoUserAlert.Title = alert.Title;
                repoUserAlert.Entries.Clear();

                var lockObject = new Object();
                var alertProducts = new List<MonitoredProduct>();
                await Task.WhenAll(alert.Entries.Select(async entry => 
                {
                    var productUrl = new Uri(entry.OriginalUrl);
                    var existingProduct = await ForceGetProduct(productUrl);

                    lock (lockObject) 
                    {
                        if (!entry.IsDeleted) 
                        {
                            alertProducts.Add(existingProduct);
                        }

                        repoUserAlert.Entries.Add(
                            new UserAlertEntry 
                            {
                                MonitoredProductId = existingProduct.Id,
                                IsDeleted = entry.IsDeleted
                            });
                    }
                }));

                var bestDeal = alertProducts
                    .Select(p => Tuple.Create(p, p.PriceHistory.LastOf(y => y.ModifiedAt)))
                    .FirstOf(x => x.Item2.Price);

                repoUserAlert.BestCurrentDeal = new Deal { ProductId = bestDeal.Item1.Id, Price = bestDeal.Item2.Price, ModifiedAt = bestDeal.Item2.ModifiedAt };

                repoUserAlert = await this._alertRepository.UpdateAsync(userId, repoUserAlert);

                var userAlert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

                return this.Ok(userAlert);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFound("The specified source is not yet supported.");
            }
            catch (ParseException e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpDelete("{userId}/{alertId}")]
        public async Task<bool> DeleteAlert(string userId, string alertId)
        {
            var isDeleted = await this._alertRepository.DeleteAsync(userId, alertId);

            return isDeleted;
        }

        private async Task<MonitoredProduct> ForceGetProduct(Uri url)
        {
            var cleanUrl = this._handlerFactory.CreateHandler(url).HandleCleanUrl(url);
            var existingProduct = await this._productRepository.GetByUrlAsync(cleanUrl.AbsoluteUri);
            if (existingProduct == null)
            {
                existingProduct = await this._productFactory.CreateProduct(url);
            }

            return existingProduct;
        }
    }
}
