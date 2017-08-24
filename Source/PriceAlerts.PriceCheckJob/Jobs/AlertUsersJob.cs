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

                        var newBestDeal = alertProducts
                            .Select(p => Tuple.Create(p, p.PriceHistory.LastOf(y => y.ModifiedAt)))
                            .FirstOf(x => x.Item2.Price);

                        if (alert.BestCurrentDeal.Price != newBestDeal.Item2.Price)
                        {
                            var changePrice = Math.Abs(alert.BestCurrentDeal.Price - newBestDeal.Item2.Price);
                            var changeAcceptationThreshold = (user.Settings.ChangePercentage * alert.BestCurrentDeal.Price);

                            // Console.WriteLine($"Price changed for alert {alert.Id} from {alert.BestCurrentDeal.Price} to {newBestDeal.Item2.Price}");
                            // Console.WriteLine($"A change of {changePrice} over a {changeAcceptationThreshold} threshold");

                            if (!user.Settings.SpecifyChangePercentage || 
                                user.Settings.SpecifyChangePercentage && changePrice > changeAcceptationThreshold)
                            {
                                if (alert.BestCurrentDeal.Price < newBestDeal.Item2.Price && user.Settings.AlertOnPriceRaise
                                || alert.BestCurrentDeal.Price > newBestDeal.Item2.Price && user.Settings.AlertOnPriceDrop)
                                {
                                    var emailAlert = new PriceChangeAlert
                                    {
                                        FirstName = user.FirstName ?? string.Empty,
                                        EmailAddress = user.Email,
                                        AlertTitle = alert.Title ?? string.Empty, 
                                        PreviousPrice = alert.BestCurrentDeal.Price, 
                                        NewPrice = newBestDeal.Item2.Price,
                                        ImageUrl = alert.ImageUrl.IsBase64Url() ? string.Empty : alert.ImageUrl,
                                        ProductUri = new Uri(newBestDeal.Item1.Uri)
                                    };

                                    Console.WriteLine($"Sending email to user {user.Id} for alert {alert.Id}");

                                    await this._emailSender.SendEmail(emailAlert);
                                }
                            }

                            alert.BestCurrentDeal = new Deal { ProductId = newBestDeal.Item1.Id, Price = newBestDeal.Item2.Price, ModifiedAt = newBestDeal.Item2.ModifiedAt };

                            // This makes one more database call since in the Update method we Get the user from the DB
                            var updateTask = this._alertRepository.UpdateAsync(user.UserId, alert);
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