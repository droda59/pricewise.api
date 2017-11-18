using System;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Extensions;

namespace PriceAlerts.CleaningJob.Jobs
{
    internal class CleanOriginalPricesJob : IJob
    {
        private readonly IUserRepository _userRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly IProductRepository _productRepository;

        public CleanOriginalPricesJob(IUserRepository userRepository, IAlertRepository alertRepository, IProductRepository productRepository)
        {
            this._userRepository = userRepository;
            this._alertRepository = alertRepository;
            this._productRepository = productRepository;
        }
        
        public async Task ExecuteJob()
        {
            var jobInfo = new JobInfo();
            
            var users = await this._userRepository.GetAllAsync();
            foreach (var user in users)
            {
                foreach (var alert in user.Alerts)
                {
                    foreach (var entry in alert.Entries)
                    {
                        if (entry.CreatedAt != default(DateTime) || entry.OriginalPrice != default(decimal))
                        {
                            jobInfo.Unhandled++;
                            continue;
                        }

                        try
                        {
                            var product = await this._productRepository.GetAsync(entry.MonitoredProductId);
                            var firstPrice = product.PriceHistory.FirstOf(x => x.ModifiedAt);

                            entry.CreatedAt = firstPrice.ModifiedAt;
                            entry.OriginalPrice = firstPrice.Price;

                            jobInfo.Success++;
                        }
                        catch (Exception)
                        {
                            jobInfo.Errors++;
                        }
                    }

                    await this._alertRepository.UpdateAsync(user.UserId, alert);
                }
            }
            
            Console.WriteLine($"{jobInfo.Success} alert entries were updated.");
            Console.WriteLine($"{jobInfo.Errors} alert entries had errors.");
            Console.WriteLine($"{jobInfo.Unhandled} alert entries were already set before.");
        }

        private class JobInfo
        {
            public int Errors { get; set; }
            public int Success { get; set; }
            public int Unhandled { get; set; }
        }
    }
}
