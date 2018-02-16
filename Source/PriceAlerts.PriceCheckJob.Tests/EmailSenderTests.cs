using System;
using System.Threading.Tasks;
using Autofac;
using PriceAlerts.Common.Models;
using PriceAlerts.PriceCheckJob.Emails;
using PriceAlerts.PriceCheckJob.Jobs;
using Xunit;

namespace PriceAlerts.PriceCheckJob.Tests
{
    public class EmailSenderTests
    {
        private readonly IContainer _container;
        private readonly IEmailSender _emailSender;

        public EmailSenderTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new PriceAlerts.Common.AutofacModule());
            builder.RegisterModule(new PriceAlerts.PriceCheckJob.AutofacModule());

            this._container = builder.Build();

            this._emailSender = this._container.Resolve<IEmailSender>();
            this._emailSender.Initialize();
        }
        
        [Theory]
        [InlineData("en", 30, 20)]
        [InlineData("en", 20, 30)]
        [InlineData("fr", 30, 20)]
        [InlineData("fr", 20, 30)]
        public async Task SendProductEmail(string correspondenceLanguage, decimal oldPrice, decimal newPrice)
        {
            var user = new User
            {
                Email = "swcommander@gmail.com",
                FirstName = "Maxime",
                Settings = new Settings { CorrespondenceLanguage = correspondenceLanguage }
            };

            var alert = new UserAlert
            {
                Id = "123456",
                Title = "Test Alert",
                ImageUrl = "https://pricewi.se/images/pricewise-logo.png",
                BestCurrentDeal = new Deal
                {
                    ProductId = "Product1",
                    Price = oldPrice,
                    ModifiedAt = DateTime.Today.AddDays(-1)
                }
            };
            
            var newBestDealProduct = new MonitoredProduct { Uri = "https://www.amazon.ca/dp/B073W77D5B/" };
            
            var job = this._container.Resolve<AlertUsersJob>();
            var emailInformation = job.BuildPriceChangeEmail(user, alert, newBestDealProduct, newPrice);
            
            Console.WriteLine($"Sending email {emailInformation.TemplateName} to user {user.FirstName} for alert {alert.Title}");

            var sendEmailTask = await this._emailSender.SendEmail(emailInformation);
            
            Assert.NotEmpty(sendEmailTask.MessageID);
            Assert.NotEmpty(sendEmailTask.TransactionID);
        }
        
        [Theory]
//        [InlineData("en", 30, 20)]
//        [InlineData("en", 20, 30)]
        [InlineData("fr", 30, 20)]
//        [InlineData("fr", 20, 30)]
        public async Task SendWatchedListProductEmail(string correspondenceLanguage, decimal oldPrice, decimal newPrice)
        {
            var user = new User
            {
                Email = "swcommander@gmail.com",
                FirstName = "Maxime",
                Settings = new Settings { CorrespondenceLanguage = correspondenceLanguage }
            };
            
            var listUser = new User { FirstName = "Toto" };
            var list = new List { Name = "Legos", Id = "9876" };

            var alert = new UserAlert
            {
                Id = "123456",
                Title = "Test Alert",
                ImageUrl = "https://pricewi.se/images/pricewise-logo.png",
                BestCurrentDeal = new Deal
                {
                    ProductId = "Product1",
                    Price = oldPrice,
                    ModifiedAt = DateTime.Today.AddDays(-1)
                }
            };
            
            var newBestDealProduct = new MonitoredProduct { Uri = "https://www.amazon.ca/dp/B073W77D5B/" };
            
            var job = this._container.Resolve<AlertUsersJob>();
            var emailInformation = job.BuildWatchedListProductEmail(user, list, listUser, alert, newBestDealProduct, newPrice);
            
            Console.WriteLine($"Sending email {emailInformation.TemplateName} to user {user.FirstName} for alert {alert.Title}");

            var sendEmailTask = await this._emailSender.SendEmail(emailInformation);
            
            Assert.NotEmpty(sendEmailTask.MessageID);
            Assert.NotEmpty(sendEmailTask.TransactionID);
        }
    }
}