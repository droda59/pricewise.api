using System;
using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Common;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Models;
using PriceAlerts.PriceCheckJob.Emails;
using PriceAlerts.PriceCheckJob.Models;

namespace PriceAlerts.PriceCheckJob.Jobs
{
    internal class AlertUsersJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly IEmailSender _emailSender;

        public AlertUsersJob(IProductRepository productRepository, IUserRepository userRepository, IAlertRepository alertRepository, IEmailSender emailSender)
        {
            this._productRepository = productRepository;
            this._userRepository = userRepository;
            this._alertRepository = alertRepository;
            this._emailSender = emailSender;
        }
        
        public async Task SendAlerts()
        {
            this._emailSender.Initialize();

            var userTask = this._userRepository.GetAllAsync();
            var productTask = this._productRepository.GetAllAsync();

            await Task.WhenAll(userTask, productTask);

            var allUsers = userTask.Result;
            var allProducts = productTask.Result.ToDictionary(x => x.Id);

            await Task.WhenAll(allUsers.Select(async user => 
            {
                try 
                {
                    Console.WriteLine("Starting user " + user.UserId);

                    var alertsInUse = user.Alerts.Where(x => !x.IsDeleted && x.IsActive).ToList();
                    foreach (var alert in alertsInUse)
                    {
                        var alertProducts = alert.Entries.Where(x => !x.IsDeleted).Select(x => allProducts[x.MonitoredProductId]).ToList();

                        var bestCurrentDeal = alert.BestCurrentDeal;
                        var bestCurrentDealUri = alertProducts.First().Uri;
                        foreach(var product in alertProducts)
                        {
                            var lastPrice = product.PriceHistory.LastOf(y => y.ModifiedAt);
                            if (lastPrice.Price < bestCurrentDeal.Price)
                            {
                                bestCurrentDeal = new Deal { ProductId = product.Id, Price = lastPrice.Price, ModifiedAt = lastPrice.ModifiedAt };
                                bestCurrentDealUri = product.Uri;
                            }
                        }

                        if (alert.BestCurrentDeal.Price != bestCurrentDeal.Price)
                        {
                            Console.WriteLine("Price dropped for alert " + alert.Id + " from " + alert.BestCurrentDeal.Price + " to " + bestCurrentDeal.Price);

                            var emailAlert = new PriceChangeAlert
                            {
                                FirstName = user.FirstName,
                                EmailAddress = user.Email,
                                AlertTitle = alert.Title, 
                                PreviousPrice = alert.BestCurrentDeal.Price, 
                                NewPrice = bestCurrentDeal.Price,
                                ProductUri = new Uri(bestCurrentDealUri)
                            };

                            alert.BestCurrentDeal = bestCurrentDeal;

                            // This makes one more database call since in the Update method we Get the user from the DB
                            var updateTask = this._alertRepository.UpdateAsync(user.UserId, alert);
                            var emailTask = this._emailSender.SendEmail(emailAlert);

                            await Task.WhenAll(updateTask, emailTask);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Found error on user " + user.UserId + ": " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Finished user " + user.UserId);
                }
            }));

            this._emailSender.Dispose();
        }
    }
}