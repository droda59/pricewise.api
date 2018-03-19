using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PriceAlerts.Common.Commands.Clients;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class SportsExpertsParser : BaseHtmlParser
    {
        private readonly SportsExpertsClient _apiClient;

        public SportsExpertsParser(IDocumentLoader documentLoader, SportsExpertsSource source)
            : base(documentLoader, source)
        {
            this._apiClient = new SportsExpertsClient();
        }
        
        protected override string GetTitle(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var productDetail = documentNode.SelectSingleNode(".//div[@class='product-detail']");
            var productDetailName = productDetail.SelectSingleNode(".//span[@class='product-detail-name']");
            var title = productDetailName.InnerText;
            
            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var productDetail = documentNode.SelectSingleNode(".//div[@class='product-detail']");
            var producDetailsMedia = productDetail.SelectSingleNode(".//div[@class='product-details-media']");
            var imageNode = producDetailsMedia.SelectSingleNode(".//img[@class='img-fluid']");
                
            var extractedValue = imageNode.Attributes["src"].Value;

            return extractedValue;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var urlMetaNode = doc.DocumentNode.SelectSingleNode(".//meta[contains(@name, 'og:url')]");
            var productIds = urlMetaNode.Attributes["content"].Value.Split('/').Reverse().Take(2).ToList();
            var productId = productIds.First(x => !x.Contains("-"));
            var variantId = productIds.First(x => x.Contains("-")) ?? productId;

            var task = this._apiClient.GetPrices(new ProductList(new[]{productId}));
            task.Wait();
            var result = task.Result;

            var variantPrices = result["ProductPrices"][0]["VariantPrices"];

            var price = (string)variantPrices.First(x => ((string) x["VariantId"]) == variantId)["ListPrice"];
            var decimalValue = price.ExtractDecimal();

            return decimalValue;
        }
    }
}