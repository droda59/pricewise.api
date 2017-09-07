using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nager.AmazonProductAdvertising;
using Nager.AmazonProductAdvertising.Model;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    internal class AmazonApiInspector : IInspector
    {
        private const string AccessKey = "AKIAIBE5OUZPPCBIESVA";
        private const string SecretKey = "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";
        
        private readonly AmazonWrapper _apiWwrapper;
        private readonly AmazonSource _source;

        public AmazonApiInspector(AmazonSource source)
        {
            this._source = source;
            
            var authentication = new AmazonAuthentication
            {
                AccessKey = AccessKey,
                SecretKey = SecretKey
            };
            
            this._apiWwrapper = new AmazonWrapper(authentication, AmazonEndpoint.CA);
        }

        public async Task<SitePriceInfo> GetSiteInfo(Uri url)
        {
            var asin = this._source.AsinExpression.Match(url.AbsoluteUri).Value;
            var result = await this._apiWwrapper.LookupAsync(asin, 
                AmazonResponseGroup.OfferSummary
                | AmazonResponseGroup.Offers 
                | AmazonResponseGroup.ItemAttributes 
                | AmazonResponseGroup.Medium);

            var item = result.Items.Item.FirstOrDefault();
            if (item == null)
            {
                Console.WriteLine($"No price available for {url}");
                return null;
            }

            decimal minPrice = 0m;
            var prices = new List<Price>();

            Console.WriteLine();
            Console.WriteLine($"Prices for {url}");

            // if (item.ItemAttributes.ListPrice != null && !string.IsNullOrEmpty(item.ItemAttributes.ListPrice.Amount))
            // {
            //     prices.Add(item.ItemAttributes.ListPrice);
            //     Console.WriteLine($"List: {item.ItemAttributes.ListPrice.Amount}, {item.ItemAttributes.ListPrice.FormattedPrice}");
            // }

            if (item.Offers.Offer.Any())
            {
                foreach (var offer in item.Offers.Offer)
                {
                    foreach (var listing in offer.OfferListing)
                    {
                        if (listing.Price != null && !string.IsNullOrEmpty(listing.Price.Amount))
                        {
                            prices.Add(listing.Price);
                            Console.WriteLine($"Listing price: {listing.Price.Amount}, {listing.Price.FormattedPrice}");
                        }

                        if (listing.SalePrice != null && !string.IsNullOrEmpty(listing.SalePrice.Amount))
                        {
                            prices.Add(listing.SalePrice);
                            Console.WriteLine($"Lowest New: {listing.SalePrice.Amount}, {listing.SalePrice.FormattedPrice}");
                        }
                    }
                }
            }
            else if (item.OfferSummary.LowestNewPrice != null && !string.IsNullOrEmpty(item.OfferSummary.LowestNewPrice.Amount))
            {
                prices.Add(item.OfferSummary.LowestNewPrice);
                Console.WriteLine($"Lowest New: {item.OfferSummary.LowestNewPrice.Amount}, {item.OfferSummary.LowestNewPrice.FormattedPrice}");
            }
            else if (item.OfferSummary.LowestUsedPrice != null && !string.IsNullOrEmpty(item.OfferSummary.LowestUsedPrice.Amount))
            {
                prices.Add(item.OfferSummary.LowestUsedPrice);
                Console.WriteLine($"Lowest Used: {item.OfferSummary.LowestUsedPrice.Amount}, {item.OfferSummary.LowestUsedPrice.FormattedPrice}");
            }

            // if (item.OfferSummary.LowestRefurbishedPrice != null && !string.IsNullOrEmpty(item.OfferSummary.LowestRefurbishedPrice.Amount))
            // {
            //     prices.Add(item.OfferSummary.LowestRefurbishedPrice);
            //     Console.WriteLine($"Lowest Refurbished: {item.OfferSummary.LowestRefurbishedPrice.Amount}, {item.OfferSummary.LowestRefurbishedPrice.FormattedPrice}");
            // }

            // if (item.OfferSummary.LowestCollectiblePrice != null && !string.IsNullOrEmpty(item.OfferSummary.LowestCollectiblePrice.Amount))
            // {
            //     prices.Add(item.OfferSummary.LowestCollectiblePrice);
            //     Console.WriteLine($"Lowest Collectible: {item.OfferSummary.LowestCollectiblePrice.Amount}, {item.OfferSummary.LowestCollectiblePrice.FormattedPrice}");
            // }

            if (prices.Any())
            {
                minPrice = prices.Min(x => x.FormattedPrice.ExtractDecimal());
                Console.WriteLine($"Min price {minPrice}");
            }

            Console.WriteLine();

            if (prices.Any())
            {
                return new SitePriceInfo
                {
                    ProductIdentifier = item.ASIN, 
                    Uri = url.AbsoluteUri,
                    Title = item.ItemAttributes.Title,
                    ImageUrl = item.MediumImage.URL,
                    Price = minPrice
                };
            }

            return null;
        }
    }
}