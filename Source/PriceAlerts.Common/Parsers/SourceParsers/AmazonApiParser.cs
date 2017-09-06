using System;
using System.Threading.Tasks;

using Nager.AmazonProductAdvertising;
using Nager.AmazonProductAdvertising.Model;

using PriceAlerts.Common.Parsers.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    public class AmazonApiParser : IApifulParser
    {
        private const string AccessKey = "AKIAIBE5OUZPPCBIESVA";
        private const string SecretKey = "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";
        
        private readonly AmazonWrapper _apiWwrapper;
        private readonly AmazonSource _source;

        public AmazonApiParser(AmazonSource source)
        {
            this._source = source;
            
            var authentication = new AmazonAuthentication
            {
                AccessKey = AccessKey,
                SecretKey = SecretKey
            };
            
            this._apiWwrapper = new AmazonWrapper(authentication, AmazonEndpoint.CA);
        }

        public ISource Source => this._source;

        public async Task<SitePriceInfo> GetSiteInfo(Uri url)
        {
            var asin = this._source.AsinExpression.Match(url.AbsoluteUri).Value;
            var result = await this._apiWwrapper.LookupAsync(asin, AmazonResponseGroup.OfferSummary | AmazonResponseGroup.ItemAttributes | AmazonResponseGroup.Medium);
            var item = result.Items.Item[0];
            
            Console.WriteLine($"Prices for {url}");
            Console.WriteLine($"List: {item.ItemAttributes.ListPrice.Amount}, {item.ItemAttributes.ListPrice.FormattedPrice}");
            Console.WriteLine($"Lowest New: {item.OfferSummary.LowestNewPrice.Amount}, {item.OfferSummary.LowestNewPrice.FormattedPrice}");
            Console.WriteLine($"Lowest Used: {item.OfferSummary.LowestUsedPrice.Amount}, {item.OfferSummary.LowestUsedPrice.FormattedPrice}");
            Console.WriteLine($"Lowest Refurbished: {item.OfferSummary.LowestRefurbishedPrice.Amount}, {item.OfferSummary.LowestRefurbishedPrice.FormattedPrice}");
            Console.WriteLine($"Lowest Collectible: {item.OfferSummary.LowestCollectiblePrice.Amount}, {item.OfferSummary.LowestCollectiblePrice.FormattedPrice}");
            Console.WriteLine();

            return new SitePriceInfo
            {
                ProductIdentifier = item.ASIN, 
                Uri = url.AbsoluteUri,
                Title = item.ItemAttributes.Title,
                ImageUrl = item.MediumImage.URL,
                Price = item.OfferSummary.LowestNewPrice.FormattedPrice.ExtractDecimal()
            };
        }
    }
}