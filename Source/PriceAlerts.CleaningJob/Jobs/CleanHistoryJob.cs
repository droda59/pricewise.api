using System;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Database;

namespace PriceAlerts.CleaningJob.Jobs
{
    internal class CleanHistoryJob : IJob
    {
        private readonly IProductRepository _productRepository;

        public CleanHistoryJob(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }
        
        public async Task ExecuteJob()
        {
            var allProducts = await this._productRepository.GetAllAsync();

            var jobInfo = new JobInfo();
            foreach (var product in allProducts)
            {
                var removedForProduct = 0;
//                Console.WriteLine($"Checking product {product.Id}");
                try
                {
                    var productChanged = false;
                    var historyByDate = product.PriceHistory.ToLookup(x => x.ModifiedAt.Date);
                    foreach (var date in historyByDate)
                    {   
                        var entriesForDate = date.OrderBy(x => x.ModifiedAt).ToList();
                        
//                        Console.WriteLine($"Checking for {date.Key.Date} with {entriesForDate.Count} entries");
                        
                        for (var i = 1; i < entriesForDate.Count; i++)
                        {
                            if (entriesForDate[i - 1].Price == entriesForDate[i].Price)
                            {
                                productChanged = true;
                                product.PriceHistory.Remove(entriesForDate[i]);
                                jobInfo.Success++;
                                removedForProduct++;
                            }
                        }
                    }

                    if (productChanged)
                    {
                        Console.WriteLine($"Saving product {product.Id} with {removedForProduct} removals, {product.PriceHistory.Count} remaining.");
                        await this._productRepository.UpdateAsync(product.Id, product);
                    }
                }
                catch (Exception)
                {
                    jobInfo.Errors++;
                }
            }
            
            Console.WriteLine($"{jobInfo.Success} entries were successfully removed from history.");
            Console.WriteLine($"{jobInfo.Errors} errors were found during cleanup of history.");
        }

        private class JobInfo
        {
            public int Errors { get; set; }
            public int Success { get; set; }
        }
    }
}