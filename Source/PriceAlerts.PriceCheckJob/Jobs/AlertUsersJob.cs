using System;
using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Common.Database;
using PriceAlerts.PriceCheckJob.Emails;
using PriceAlerts.PriceCheckJob.Models;

namespace PriceAlerts.PriceCheckJob.Jobs
{
    internal class AlertUsersJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;

        public AlertUsersJob(IProductRepository productRepository, IUserRepository userRepository, IEmailSender emailSender)
        {
            this._productRepository = productRepository;
            this._userRepository = userRepository;
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
                            var lastPrice = product.PriceHistory.OrderBy(y => y.ModifiedAt).Last();
                            if (lastPrice.Price < bestCurrentDeal.Price)
                            {
                                bestCurrentDeal = lastPrice;
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
                            alert.LastModifiedAt = DateTime.Now;

                            var updateTask = this._userRepository.UpdateAsync(user.UserId, user);
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