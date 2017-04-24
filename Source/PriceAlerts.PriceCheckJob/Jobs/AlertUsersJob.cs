using System;
using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Common.Database;

namespace PriceAlerts.PriceCheckJob.Jobs
{
    internal class AlertUsersJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public AlertUsersJob(IProductRepository productRepository, IUserRepository userRepository)
        {
            this._productRepository = productRepository;
            this._userRepository = userRepository;
        }
        
        public async Task SendAlerts()
        {
            var userTask = this._userRepository.GetAllAsync();
            var productTask = this._productRepository.GetAllAsync();

            await Task.WhenAll(userTask, productTask);

            var allUsers = userTask.Result;
            var allProducts = productTask.Result.ToDictionary(x => x.Id);

            await Task.WhenAll(allUsers.Select(async user => 
            {
                Console.WriteLine("Starting user " + user.UserId);

                var alertsInUse = user.Alerts.Where(x => !x.IsDeleted && x.IsActive).ToList();
                foreach (var alert in alertsInUse)
                {
                    var alertProducts = alert.Entries.Where(x => !x.IsDeleted).Select(x => allProducts[x.MonitoredProductId]).ToList();

                    var bestCurrentDeal = alert.BestCurrentDeal;
                    foreach(var product in alertProducts)
                    {
                        var lastPrice = product.PriceHistory.OrderBy(y => y.ModifiedAt).Last();
                        if (lastPrice.Price < bestCurrentDeal.Price)
                        {
                            bestCurrentDeal = lastPrice;
                        }
                    }

                    Console.WriteLine("Best price for alert " + alert.Id + " is " + bestCurrentDeal.Price);

                    if (alert.BestCurrentDeal.Price != bestCurrentDeal.Price)
                    {
                        Console.WriteLine("Price dropped for alert " + alert.Id + " from " + alert.BestCurrentDeal.Price + " to " + bestCurrentDeal.Price);

                        alert.BestCurrentDeal = bestCurrentDeal;
                        await this._userRepository.UpdateAsync(user.UserId, user);
                    }
                }

                Console.WriteLine("Finished user " + user.UserId);
            }));
        }
    }
}