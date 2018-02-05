using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Common.Database;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Factories;
using PriceAlerts.Common.Models;
using PriceAlerts.PriceCheckJob.Emails;

namespace PriceAlerts.PriceCheckJob.Jobs
{
    internal class AlertUsersJob : IDisposable
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IListRepository _listRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly IHandlerFactory _handlerFactory;
        private readonly IEmailSender _emailSender;

        public AlertUsersJob(
            IProductRepository productRepository,
            IUserRepository userRepository,
            IListRepository listRepository, 
            IAlertRepository alertRepository,
            IHandlerFactory handlerFactory,
            IEmailSender emailSender)
        {
            this._productRepository = productRepository;
            this._userRepository = userRepository;
            this._listRepository = listRepository;
            this._alertRepository = alertRepository;
            this._handlerFactory = handlerFactory;
            this._emailSender = emailSender;
        }

        public async Task SendAlerts()
        {
            this._emailSender.Initialize();

            var userTask = this._userRepository.GetAllAsync();
            var productTask = this._productRepository.GetAllAsync();

            await Task.WhenAll(userTask, productTask);

            var allUsers = userTask.Result;
            var productsById = productTask.Result.ToDictionary(x => x.Id);

            foreach (var user in allUsers)
            {
                await this.CheckUserAlerts(user, productsById);
            }
        }

        private async Task CheckUserAlerts(User user, IDictionary<string, MonitoredProduct> productsById)
        {
            try
            {   
                var alertsInUse = user.Alerts.Where(x => !x.IsDeleted && x.IsActive).ToList();
                foreach (var alert in alertsInUse)
                {
                    var alertProducts = alert.Entries.Where(x => !x.IsDeleted).Select(x => productsById[x.MonitoredProductId]).ToList();

                    var newBestDeal = alertProducts
                        .Select(p => Tuple.Create(p, p.PriceHistory.LastOf(y => y.ModifiedAt)))
                        .FirstOf(x => x.Item2.Price);

                    if (alert.BestCurrentDeal.Price != newBestDeal.Item2.Price)
                    {
                        var tasksToRun = new List<Task>();
                        var changePrice = Math.Abs(alert.BestCurrentDeal.Price - newBestDeal.Item2.Price);
                        var changeAcceptationThreshold = user.Settings.ChangePercentage * alert.BestCurrentDeal.Price;

                        // Console.WriteLine($"Price changed for alert {alert.Id} from {alert.BestCurrentDeal.Price} to {newBestDeal.Item2.Price}");
                        // Console.WriteLine($"A change of {changePrice} over a {changeAcceptationThreshold} threshold");

                        if (!user.Settings.SpecifyChangePercentage ||
                            user.Settings.SpecifyChangePercentage && changePrice > changeAcceptationThreshold)
                        {
                            if (alert.BestCurrentDeal.Price < newBestDeal.Item2.Price && user.Settings.AlertOnPriceRaise
                            || alert.BestCurrentDeal.Price > newBestDeal.Item2.Price && user.Settings.AlertOnPriceDrop)
                            {
                                var productUrl = new Uri(newBestDeal.Item1.Uri);
                                var commandHandler = this._handlerFactory.CreateHandler(productUrl);
                                var cleanUrl = commandHandler.HandleCleanUrl(productUrl);
                                var manipulatedUrl = commandHandler.HandleManipulateUrl(cleanUrl);
                                var subject = GetProductTitle(user, alert);

                                var parameters = new Dictionary<string, string>
                                {
                                    { "subject", user.Settings.CorrespondenceLanguage == "en"
                                        ? $"Price alert from PriceWise: The price of {subject} changed!"
                                        : $"Alerte de prix de PriceWise: Le prix de {subject} a changé!"
                                    },
                                    { "merge_firstname" , user.FirstName ?? string.Empty },
                                    { "merge_productName" , alert.Title ?? string.Empty },
                                    { "merge_previousPrice" , alert.BestCurrentDeal.Price.ToString(CultureInfo.InvariantCulture) },
                                    { "merge_newPrice" , newBestDeal.Item2.Price.ToString(CultureInfo.InvariantCulture) },
                                    { "merge_alertId", alert.Id },
                                    { "merge_productUrl" , manipulatedUrl.AbsoluteUri },
                                    { "merge_productDomain", productUrl.Authority },
                                    { "merge_imageUrl", alert.ImageUrl.IsBase64Url() ? string.Empty : alert.ImageUrl }
                                };

                                var templateName = "Drop";
                                if(alert.BestCurrentDeal.Price < newBestDeal.Item2.Price)
                                {
                                    templateName = "Raise";
                                }
                                var emailTemplate = $"Price{templateName}_Unsubscribe_{user.Settings.CorrespondenceLanguage}";

                                Console.WriteLine($"Sending email {emailTemplate} to user {user.Id} for alert {alert.Id}");

                                var sendEmailTask = this._emailSender.SendEmail(user.Email, parameters, emailTemplate);
                                tasksToRun.Add(sendEmailTask);
                            }
                        }

                        alert.BestCurrentDeal = new Deal { ProductId = newBestDeal.Item1.Id, Price = newBestDeal.Item2.Price, ModifiedAt = newBestDeal.Item2.ModifiedAt };

                        // This makes one more database call since in the Update method we Get the user from the DB
                        var updateTask = this._alertRepository.UpdateAsync(user.UserId, alert);
                        tasksToRun.Add(updateTask);

                        Task.WaitAll(tasksToRun.ToArray());
                    }
                }
                
                var userWatchedLists = await this._listRepository.GetUserWatchedAlertListsAsync(user.UserId);
                foreach (var watchedList in userWatchedLists)
                {
                    foreach (var alert in watchedList.Alerts.Where(x => !x.IsDeleted && x.IsActive))
                    {
                        var alertProducts = alert.Entries.Where(x => !x.IsDeleted).Select(x => productsById[x.MonitoredProductId]).ToList();
    
                        var newBestDeal = alertProducts
                            .Select(p => Tuple.Create(p, p.PriceHistory.LastOf(y => y.ModifiedAt)))
                            .FirstOf(x => x.Item2.Price);
    
                        if (alert.BestCurrentDeal.Price != newBestDeal.Item2.Price)
                        {
                            var tasksToRun = new List<Task>();
                            var changePrice = Math.Abs(alert.BestCurrentDeal.Price - newBestDeal.Item2.Price);
                            var changeAcceptationThreshold = user.Settings.ChangePercentage * alert.BestCurrentDeal.Price;
    
                            // Console.WriteLine($"Price changed for alert {alert.Id} from {alert.BestCurrentDeal.Price} to {newBestDeal.Item2.Price}");
                            // Console.WriteLine($"A change of {changePrice} over a {changeAcceptationThreshold} threshold");
    
                            if (!user.Settings.SpecifyChangePercentage ||
                                user.Settings.SpecifyChangePercentage && changePrice > changeAcceptationThreshold)
                            {
                                if (alert.BestCurrentDeal.Price < newBestDeal.Item2.Price && user.Settings.AlertOnPriceRaise
                                || alert.BestCurrentDeal.Price > newBestDeal.Item2.Price && user.Settings.AlertOnPriceDrop)
                                {
                                    var productUrl = new Uri(newBestDeal.Item1.Uri);
                                    var commandHandler = this._handlerFactory.CreateHandler(productUrl);
                                    var cleanUrl = commandHandler.HandleCleanUrl(productUrl);
                                    var manipulatedUrl = commandHandler.HandleManipulateUrl(cleanUrl);
                                    var listUser = await this._userRepository.GetAsync(watchedList.UserId);
                                    var subject = GetProductTitle(user, alert);

                                    var parameters = new Dictionary<string, string>
                                    {
                                        { "subject", user.Settings.CorrespondenceLanguage == "en"
                                            ? $"Price alert from PriceWise: The price of {subject} in a list you are watching changed!"
                                            : $"Alerte de prix de PriceWise: Le prix de {subject} dans une liste que vous surveillez a changé!"
                                        },
                                        { "merge_listUserName" , listUser.FirstName}, 
                                        { "merge_firstname" , user.FirstName ?? string.Empty },
                                        { "merge_productName" , alert.Title ?? string.Empty },
                                        { "merge_previousPrice" , alert.BestCurrentDeal.Price.ToString(CultureInfo.InvariantCulture) },
                                        { "merge_newPrice" , newBestDeal.Item2.Price.ToString(CultureInfo.InvariantCulture) },
                                        { "merge_alertId", alert.Id },
                                        { "merge_listId", watchedList.Id },
                                        { "merge_productUrl" , manipulatedUrl.AbsoluteUri },
                                        { "merge_productDomain", productUrl.Authority },
                                        { "merge_imageUrl", alert.ImageUrl.IsBase64Url() ? string.Empty : alert.ImageUrl }
                                    };
    
                                    var templateName = "Drop";
                                    if(alert.BestCurrentDeal.Price < newBestDeal.Item2.Price)
                                    {
                                        templateName = "Raise";
                                    }
                                    var emailTemplate = $"WatchedListPrice{templateName}_{user.Settings.CorrespondenceLanguage}";
    
                                    Console.WriteLine($"Sending email {emailTemplate} to user {user.Id} for alert {alert.Id}");
                                    
                                    var sendEmailTask = this._emailSender.SendEmail(user.Email, parameters, emailTemplate);
                                    tasksToRun.Add(sendEmailTask);
                                }
                            }
    
                            alert.BestCurrentDeal = new Deal { ProductId = newBestDeal.Item1.Id, Price = newBestDeal.Item2.Price, ModifiedAt = newBestDeal.Item2.ModifiedAt };
    
                            // This makes one more database call since in the Update method we Get the user from the DB
                            var updateTask = this._alertRepository.UpdateAsync(user.UserId, alert);
                            tasksToRun.Add(updateTask);
    
                            Task.WaitAll(tasksToRun.ToArray());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Found error on user " + user.UserId + ": " + e.Message);
            }
        }

        private static string GetProductTitle(User user, UserAlert alert)
        {
            var subject = alert.Title ?? (user.Settings.CorrespondenceLanguage == "en"
                              ? "a product"
                              : "un produit");
            if (subject.Length > 30)
            {
                subject = subject.Trim().Substring(0, 30) + "...";
            }
            return subject;
        }

        public void Dispose()
        {
            this._emailSender.Dispose();
        }
    }
}