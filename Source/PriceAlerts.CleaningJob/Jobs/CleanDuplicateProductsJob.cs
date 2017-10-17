using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Models;

namespace PriceAlerts.CleaningJob.Jobs
{
    internal class CleanDuplicateProductsJob : IJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAlertRepository _alertRepository;

        public CleanDuplicateProductsJob(IProductRepository productRepository, IUserRepository userRepository, IAlertRepository alertRepository)
        {
            this._productRepository = productRepository;
            this._userRepository = userRepository;
            this._alertRepository = alertRepository;
        }
        
        public async Task ExecuteJob()
        {
            var jobInfo = new JobInfo();
            
            
            var allProductsTask = this._productRepository.GetAllAsync();
            var allUsersTask = this._userRepository.GetAllAsync();

            Task.WaitAll(allProductsTask, allUsersTask);

            var duplicates = allProductsTask.Result.ToLookup(x => x.Uri);
            var allUsers = allUsersTask.Result.ToList();

            var totalDuplicates = 0;
            
            foreach (var duplicateProductEntries in duplicates.Where(x => x.Count() > 1))
            {
                var duplicateEntries = duplicateProductEntries.ToList();
                totalDuplicates += duplicateEntries.Count;
                Console.WriteLine();
                Console.WriteLine($"Found {duplicateEntries.Count} entries of {duplicateProductEntries.Key}");

                var originalEntry = duplicateEntries[0];
                for (var i = 1; i < duplicateEntries.Count; i++)
                {
                    var duplicateEntry = duplicateEntries[i];
                    
                    if (string.IsNullOrWhiteSpace(originalEntry.ImageUrl))
                    {
                        originalEntry.ImageUrl = duplicateEntry.ImageUrl;
                    }
                    
                    if (string.IsNullOrWhiteSpace(originalEntry.Title))
                    {
                        originalEntry.Title = duplicateEntry.Title;
                    }
                    
                    if (string.IsNullOrWhiteSpace(originalEntry.ProductIdentifier))
                    {
                        originalEntry.ProductIdentifier = duplicateEntry.ProductIdentifier;
                    }

                    ((List<PriceChange>)originalEntry.PriceHistory).AddRange(duplicateEntry.PriceHistory);

                    try
                    {
                        var isInUse = false;
                        foreach (var user in allUsers)
                        {
                            foreach (var alert in user.Alerts)
                            {
                                var isInUseForAlert = false;
                                foreach (var entry in alert.Entries)
                                {
                                    if (entry.MonitoredProductId == duplicateEntry.Id)
                                    {
                                        entry.MonitoredProductId = originalEntry.Id;
                                        isInUse = true;
                                        isInUseForAlert = true;
                                    }
                                }
    
                                if (isInUseForAlert)
                                {
                                    await this._alertRepository.UpdateAsync(user.UserId, alert);
                                    jobInfo.Updated++;
                                }
                            }
                        }
    
                        await this._productRepository.DeleteAsync(duplicateEntry.Id);
                        
                        if (isInUse)
                        {
//                            Console.WriteLine($"Deleting product {duplicateEntry.Id} replaced with original entry.");
                            jobInfo.Deleted++;
                        }
                        else
                        {
//                            Console.WriteLine($"Deleting product {duplicateEntry.Id} with no use.");
                            jobInfo.Deleted++;
                            jobInfo.Unused++;
                        }
                    }
                    catch (Exception)
                    {
                        jobInfo.Errors++;
                    }
                }
            }
            
            Console.WriteLine($"{totalDuplicates} total duplicates for {duplicates.Count(x => x.Count() > 1)} products.");
            Console.WriteLine($"{jobInfo.Unused} duplicates are not even used.");
            Console.WriteLine($"{jobInfo.Updated} alerts were updated.");
            Console.WriteLine($"{jobInfo.Deleted} duplicates were successfully removed.");
            Console.WriteLine($"{jobInfo.Errors} errors were found during cleanup of history.");
        }

        private class JobInfo
        {
            public int Updated { get; set; }
            public int Errors { get; set; }
            public int Deleted { get; set; }
            public int Unused { get; set; }
        }
    }
}