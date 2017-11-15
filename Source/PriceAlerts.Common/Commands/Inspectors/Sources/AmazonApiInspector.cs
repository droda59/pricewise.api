using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nager.AmazonProductAdvertising;
using Nager.AmazonProductAdvertising.Model;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class AmazonApiInspector : IInspector
    {
        private const string AccessKey = "AKIAJ77TFJHVOREMTPCA";
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

        [LoggingDescription("Parsing API")]
        public async Task<SitePriceInfo> GetSiteInfo(Uri url)
        {
            Item item;
            var asin = this._source.AsinExpression.Match(url.AbsoluteUri).Value;
            if (asin.EndsWith('/'))
            {
                asin = asin.Replace("/", string.Empty);
            }
            
            var result = await this._apiWwrapper.LookupAsync(asin, 
                AmazonResponseGroup.OfferSummary
                | AmazonResponseGroup.Offers 
                | AmazonResponseGroup.ItemAttributes 
                | AmazonResponseGroup.Medium);

            try
            {
                item = result?.Items.Item.FirstOrDefault();
            }
            catch (Exception)
            {
                item = null;
            }
            
            if (item == null)
            {
                return null;
            }

            var prices = new List<Price>();

            if (item.Offers.Offer.Any())
            {
                foreach (var offer in item.Offers.Offer)
                {
                    foreach (var listing in offer.OfferListing)
                    {
                        if (!string.IsNullOrEmpty(listing.Price?.Amount))
                        {
                            prices.Add(listing.Price);
                        }

                        if (!string.IsNullOrEmpty(listing.SalePrice?.Amount))
                        {
                            prices.Add(listing.SalePrice);
                        }
                    }
                }
            }

            if (!prices.Any())
            {
                return null;
            }

            var minPrice = prices.Min(x => x.FormattedPrice.ExtractDecimal());

            return new SitePriceInfo
            {
                ProductIdentifier = item.ASIN,
                Uri = url.AbsoluteUri,
                Title = item.ItemAttributes.Title,
                ImageUrl = item.MediumImage.URL,
                Price = minPrice
            };
        }
    }
}