using System;
using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Api.Models;
using PriceAlerts.Common;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Factories
{
    internal class UserAlertFactory : IUserAlertFactory
    {
        private readonly IProductRepository _productRepository;

        public UserAlertFactory(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        public async Task<UserAlertDto> CreateUserAlert(UserAlert repoAlert)
        {
            var bestDealProduct = await this._productRepository.GetAsync(repoAlert.BestCurrentDeal.ProductId);

            var userAlert = new UserAlertDto
            {
                Id = repoAlert.Id,
                Title = repoAlert.Title,
                ImageUrl = repoAlert.ImageUrl,
                IsActive = repoAlert.IsActive,
                LastModifiedAt = repoAlert.LastModifiedAt, 
                BestCurrentDeal = new DealDto 
                {
                    Price = repoAlert.BestCurrentDeal.Price,
                    Title = bestDealProduct.Title,
                    ModifiedAt = repoAlert.BestCurrentDeal.ModifiedAt,
                    ProductUrl = bestDealProduct.Uri
                }
            };

            var lockObject = new Object();    
            var notDeletedEntries = repoAlert.Entries.Where(x => !x.IsDeleted).ToList();
            await Task.WhenAll(notDeletedEntries.Select(async entry => 
            {
                var entryProduct = await this._productRepository.GetAsync(entry.MonitoredProductId);
                var lastUpdate = entryProduct.PriceHistory.LastOf(x => x.ModifiedAt);

                var userAlertEntry = new UserAlertEntryDto
                {
                    Uri = entryProduct.Uri,
                    LastPrice = lastUpdate.Price,
                    LastUpdate = lastUpdate.ModifiedAt,
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