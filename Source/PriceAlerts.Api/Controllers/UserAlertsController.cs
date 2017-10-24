using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PriceAlerts.Api.Factories;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.CommandHandlers;
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

        public UserAlertsController(IProductRepository productRepository, IAlertRepository alertRepository, IUserAlertFactory userAlertFactory, IProductFactory productFactory, IHandlerFactory handlerFactory)
        {
            this._productRepository = productRepository;
            this._alertRepository = alertRepository;
            this._userAlertFactory = userAlertFactory;
            this._productFactory = productFactory;
        }

        [HttpGet("{userId}")]
        [LoggingDescription("Request to get alert summaries")]
        public virtual async Task<IEnumerable<UserAlertSummaryDto>> GetSummaries(string userId)
        {
            var alertSummaries = new List<UserAlertSummaryDto>();
            var lockObject = new object();
            
            var userAlerts = await this._alertRepository.GetAllAsync(userId);
            await Task.WhenAll(userAlerts.Select(async repoUserAlert => 
            {
                if (!repoUserAlert.IsDeleted)
                {
                    var alert = await this._userAlertFactory.CreateUserAlertSummary(repoUserAlert);

                    lock(lockObject)
                    {
                        alertSummaries.Add(alert);
                    }  
                }
            }));

            return alertSummaries;
        }

        [HttpGet("{userId}/{alertId}")]
        [LoggingDescription("Request to get alert")]
        public virtual async Task<UserAlertDto> Get(string userId, string alertId)
        {
            var repoUserAlert = await this._alertRepository.GetAsync(userId, alertId);

            var userAlert = await this._userAlertFactory.CreateUserAlert(repoUserAlert);

            return userAlert;
        }

        [HttpGet("{userId}/{alertId}/summary")]
        [LoggingDescription("Request to get alert summary")]
        public virtual async Task<UserAlertSummaryDto> GetSummary(string userId, string alertId)
        {
            var repoUserAlert = await this._alertRepository.GetAsync(userId, alertId);

            var userAlert = await this._userAlertFactory.CreateUserAlertSummary(repoUserAlert);

            return userAlert;
        }

        [HttpGet("{userId}/{alertId}/history")]
        [LoggingDescription("Request to get alert history")]
        public virtual async Task<IEnumerable<ProductHistory>> GetHistory(string userId, string alertId)
        {
            var repoUserAlert = await this._alertRepository.GetAsync(userId, alertId);

            var lockObject = new object();
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
        [LoggingDescription("Request to create alert")]
        public virtual async Task<IActionResult> CreateAlert(string userId, [FromBody]Uri uri)
        {
            try 
            {
                var existingProduct = await this._productFactory.CreateUpdatedProduct(uri);
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

                var userAlert = await this._userAlertFactory.CreateUserAlertSummary(newAlert);

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

        [HttpPost("{userId}/{alertId}/entry")]
        [LoggingDescription("Request to create an alert entry")]
        public virtual async Task<IActionResult> CreateAlertEntry(string userId, string alertId, [FromBody]Uri uri)
        {
            try 
            {
                var repoUserAlert = await this._alertRepository.GetAsync(userId, alertId);
                var existingProduct = await this._productFactory.CreateUpdatedProduct(uri);

                repoUserAlert.Entries.Add(new UserAlertEntry { MonitoredProductId = existingProduct.Id });

                var lastProductHistoryEntry = existingProduct.PriceHistory.LastOf(x => x.ModifiedAt);
                if (lastProductHistoryEntry.Price < repoUserAlert.BestCurrentDeal.Price)
                {
                    repoUserAlert.BestCurrentDeal = new Deal
                    {
                        ProductId = existingProduct.Id, 
                        Price = lastProductHistoryEntry.Price, 
                        ModifiedAt = lastProductHistoryEntry.ModifiedAt
                    };
                }

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

        [HttpPut("{userId}/{alertId}/activate")]
        [LoggingDescription("Request to change alert active state")]
        public virtual async Task<IActionResult> ActivateAlert(string userId, string alertId, [FromBody]bool isActive)
        {
            try
            {
                var repoUserAlert = await this._alertRepository.GetAsync(userId, alertId);
                repoUserAlert.IsActive = isActive;

                await this._alertRepository.UpdateAsync(userId, repoUserAlert);

                return this.Ok(true);
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut("{userId}/summary")]
        [LoggingDescription("Request to update an alert summary")]
        public virtual async Task<IActionResult> UpdateAlertSummary(string userId, [FromBody]UserAlertSummaryDto alert)
        {
            try
            {
                var repoUserAlert = await this._alertRepository.GetAsync(userId, alert.Id);
                repoUserAlert.IsActive = alert.IsActive;
                repoUserAlert.Title = alert.Title;
                
                repoUserAlert = await this._alertRepository.UpdateAsync(userId, repoUserAlert);

                var userAlert = await this._userAlertFactory.CreateUserAlertSummary(repoUserAlert);

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
        [LoggingDescription("Request to update an alert")]
        public virtual async Task<IActionResult> UpdateAlert(string userId, [FromBody]UserAlertDto alert)
        {
            try
            {
                var repoUserAlert = await this._alertRepository.GetAsync(userId, alert.Id);
                repoUserAlert.IsActive = alert.IsActive;
                repoUserAlert.Title = alert.Title;
                repoUserAlert.Entries.Clear();

                var lockObject = new object();
                var alertProducts = new List<MonitoredProduct>();
                await Task.WhenAll(alert.Entries.Select(async entry => 
                {
                    var productUrl = new Uri(entry.OriginalUrl);
                    var existingProduct = await this._productFactory.CreateProduct(productUrl);

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
        [LoggingDescription("Request to delete an alert")]
        public virtual async Task<bool> DeleteAlert(string userId, string alertId)
        {
            var isDeleted = await this._alertRepository.DeleteAsync(userId, alertId);

            return isDeleted;
        }
    }
}
