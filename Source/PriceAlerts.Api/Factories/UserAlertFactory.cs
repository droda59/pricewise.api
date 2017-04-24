using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Common.Database;

namespace PriceAlerts.Api.Factories
{
    internal class UserAlertFactory : IUserAlertFactory
    {
        private readonly IProductRepository _productRepository;

        public UserAlertFactory(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        public async Task<Models.UserAlert> CreateUserAlert(Common.Models.UserAlert repoAlert)
        {
            var bestDealProduct = await this._productRepository.GetAsync(repoAlert.BestCurrentDeal.ProductId);

            var userAlert = new Api.Models.UserAlert
            {
                Id = repoAlert.Id,
                Title = repoAlert.Title,
                ImageUrl = repoAlert.ImageUrl,
                IsActive = repoAlert.IsActive,
                BestCurrentDeal = new Api.Models.Deal 
                {
                    Price = repoAlert.BestCurrentDeal.Price,
                    ModifiedAt = repoAlert.BestCurrentDeal.ModifiedAt,
                    ProductUrl = bestDealProduct.Uri
                }
            };

            var notDeletedEntries = repoAlert.Entries.Where(x => !x.IsDeleted).ToList();
            await Task.WhenAll(notDeletedEntries.Select(async entry => 
            {
                var entryProduct = await this._productRepository.GetAsync(entry.MonitoredProductId);
                var lastUpdate = entryProduct.PriceHistory.OrderByDescending(x => x.ModifiedAt).First();

                var userAlertEntry = new Api.Models.UserAlertEntry
                {
                    Uri = entryProduct.Uri,
                    LastPrice = lastUpdate.Price,
                    LastUpdate = lastUpdate.ModifiedAt,
                    Title = entryProduct.Title
                };

                userAlert.Entries.Add(userAlertEntry);
            }));

            return userAlert;
        }
    }
}