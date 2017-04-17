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
            var userAlert = new PriceAlerts.Api.Models.UserAlert
            {
                Id = repoAlert.Id,
                Title = repoAlert.Title,
                ImageUrl = repoAlert.ImageUrl,
                IsActive = repoAlert.IsActive
            };

            var notDeletedEntries = repoAlert.Entries.Where(x => !x.IsDeleted).ToList();
            await Task.WhenAll(notDeletedEntries.Select(async entry => 
            {
                var entryProduct = await this._productRepository.GetAsync(entry.MonitoredProductId);
                var lastUpdate = entryProduct.PriceHistory.OrderByDescending(x => x.ModifiedAt).First();

                var userAlertEntry = new PriceAlerts.Api.Models.UserAlertEntry
                {
                    Uri = entryProduct.Uri,
                    LastPrice = lastUpdate.Price,
                    LastUpdate = lastUpdate.ModifiedAt
                };

                userAlert.Entries.Add(userAlertEntry);
            }));

            userAlert.LastUpdate = userAlert.Entries.Where(x => !x.IsDeleted).Max(x => x.LastUpdate);
            userAlert.BestCurrentPrice = userAlert.Entries.Where(x => !x.IsDeleted).Min(x => x.LastPrice);

            return userAlert;
        }
    }
}