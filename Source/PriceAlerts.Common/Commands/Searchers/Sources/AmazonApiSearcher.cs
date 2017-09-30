using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nager.AmazonProductAdvertising;
using Nager.AmazonProductAdvertising.Model;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Searchers.Sources
{
    internal class AmazonApiSearcher : ISearcher
    {
        private const string AccessKey = "AKIAIBE5OUZPPCBIESVA";
        private const string SecretKey = "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";
        
        private readonly AmazonWrapper _apiWwrapper;
        private readonly AmazonSource _source;
        
        public AmazonApiSearcher(AmazonSource source)
        {
            var authentication = new AmazonAuthentication
            {
                AccessKey = AccessKey,
                SecretKey = SecretKey
            };

            this._source = source;
            this._apiWwrapper = new AmazonWrapper(authentication, AmazonEndpoint.CA);
        }

        public async Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm, int maxResultCount = 5)
        {
            var result = await this._apiWwrapper.SearchAsync(searchTerm, AmazonSearchIndex.All, AmazonResponseGroup.ItemAttributes);

            var item = result.Items.Item.Take(maxResultCount);
            
            return item.Select(x => new Uri(x.DetailPageURL));
        }

        public ISource Source => this._source;
    }
}
