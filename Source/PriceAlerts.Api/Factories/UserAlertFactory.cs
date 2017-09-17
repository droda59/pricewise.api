using System;
using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Api.Models;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Factories
{
    internal class UserAlertFactory : IUserAlertFactory
    {
        private readonly IHandlerFactory _handlerFactory;
        private readonly IProductRepository _productRepository;

        public UserAlertFactory(IHandlerFactory handlerFactory, IProductRepository productRepository)
        {
            this._handlerFactory = handlerFactory;
            this._productRepository = productRepository;
        }

        public async Task<UserAlertDto> CreateUserAlert(UserAlert repoAlert)
        {
            var bestDealProduct = await this._productRepository.GetAsync(repoAlert.BestCurrentDeal.ProductId);

            var bestDealUrl = new Uri(bestDealProduct.Uri);
            var bestDealHandler = this._handlerFactory.CreateHandler(bestDealUrl);

            var userAlert = new UserAlertDto
            {
                Id = repoAlert.Id,
                Title = repoAlert.Title,
                ImageUrl = repoAlert.ImageUrl,
                IsActive = repoAlert.IsActive,
                BestCurrentDeal = new DealDto 
                {
                    Price = repoAlert.BestCurrentDeal.Price,
                    Title = bestDealProduct.Title,
                    ModifiedAt = repoAlert.BestCurrentDeal.ModifiedAt,
                    OriginalUrl = bestDealProduct.Uri,
                    ProductUrl = bestDealHandler.HandleManipulateUrl(bestDealUrl).AbsoluteUri
                }
            };

            var lockObject = new object();
            var notDeletedEntries = repoAlert.Entries.Where(x => !x.IsDeleted).ToList();
            await Task.WhenAll(notDeletedEntries.Select(async entry => 
            {
                var entryProduct = await this._productRepository.GetAsync(entry.MonitoredProductId);
                var productUrl = new Uri(entryProduct.Uri);
                var lastUpdate = entryProduct.PriceHistory.LastOf(x => x.ModifiedAt);
                var handler = this._handlerFactory.CreateHandler(productUrl);

                var userAlertEntry = new UserAlertEntryDto
                {
                    OriginalUrl = entryProduct.Uri,
                    ProductUrl = handler.HandleManipulateUrl(productUrl).AbsoluteUri,
                    LastPrice = lastUpdate.Price,
                    Title = entryProduct.Title,
                    ProductIdentifier = entryProduct.ProductIdentifier
                };

                lock (lockObject) 
                {
                    userAlert.Entries.Add(userAlertEntry);
                }
            }));

            return userAlert;
        }
    }
}