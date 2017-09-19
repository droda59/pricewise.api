using System;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.CommandHandlers;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Factories;

namespace PriceAlerts.CleaningJob.Jobs
{
    internal class CleanUrlsJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IHandlerFactory _handlerFactory;

        public CleanUrlsJob(IProductRepository productRepository, IHandlerFactory handlerFactory)
        {
            this._productRepository = productRepository;
            this._handlerFactory = handlerFactory;
        }
        
        public async Task ExecuteJob()
        {
            var allProducts = await this._productRepository.GetAllAsync();

            var jobInfo = new JobInfo();
            foreach (var product in allProducts)
            {
                try
                {
                    var productUri = new Uri(product.Uri);
                    var cleanUrl = this._handlerFactory.CreateHandler(productUri).HandleCleanUrl(productUri);

                    if (product.Uri == cleanUrl.AbsoluteUri)
                    {
                        jobInfo.Unhandled++;
                        continue;
                    }

//                    Console.WriteLine($"{product.Uri} became {cleanUrl.AbsoluteUri}");
                    product.Uri = cleanUrl.AbsoluteUri;

                    await this._productRepository.UpdateAsync(product.Id, product);

                    jobInfo.Success++;
                }
                catch (Exception)
                {
//                    Console.WriteLine($"{product.Uri} had error: {e.Message}");
                    jobInfo.Errors++;
                }
            }
            
            Console.WriteLine($"{jobInfo.Success} urls were successfully cleaned.");
            Console.WriteLine($"{jobInfo.Errors} urls had errors.");
            Console.WriteLine($"{jobInfo.Unhandled} urls were unhandled.");
        }

        private class JobInfo
        {
            public int Errors { get; set; }
            public int Success { get; set; }
            public int Unhandled { get; set; }
        }
    }
}