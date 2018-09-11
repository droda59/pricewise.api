using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nager.AmazonProductAdvertising;
using Nager.AmazonProductAdvertising.Model;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Common.Commands.Searchers.Sources
{
    public class AmazonApiSearcher : ISearcher
    {
        private const string AccessKey = "AKIAI2USA5KCUXZQ32CQ";
        private const string SecretKey = "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";
        
        private readonly AmazonWrapper _apiWwrapper;

        public AmazonApiSearcher()
        {
            var authentication = new AmazonAuthentication
            {
                AccessKey = AccessKey,
                SecretKey = SecretKey
            };

            this._apiWwrapper = new AmazonWrapper(authentication, AmazonEndpoint.CA);
        }

        [LoggingDescription("Searching API for URLs")]
        public virtual async Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm, int maxResultCount = 3)
        {
            var result = await this._apiWwrapper.SearchAsync(searchTerm, AmazonSearchIndex.All, AmazonResponseGroup.ItemAttributes);

            if (result == null)
            {
                return Enumerable.Empty<Uri>();
            }
            
            var item = result.Items.Item.Take(maxResultCount);
            
            return item.Select(x => 
            {
                var detailPageURL = x.DetailPageURL;
                if (!detailPageURL.EndsWith('/'))
                {
                    detailPageURL = detailPageURL += "/";
                }

                return new Uri(detailPageURL);
            });
        }
    }
}
