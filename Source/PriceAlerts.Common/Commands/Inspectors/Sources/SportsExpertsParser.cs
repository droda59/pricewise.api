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
            var titleMeta = documentNode.SelectSingleNode(".//meta[@name='og:title']");
            var title = titleMeta.Attributes["content"].Value;
            
            var extractedValue = title.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var imgMeta = documentNode.SelectSingleNode(".//meta[@name='og:image']");
            var imgUrl = imgMeta.Attributes["content"].Value;

            return imgUrl;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var documentNode = doc.DocumentNode;
            var urlMeta = documentNode.SelectSingleNode(".//meta[@name='og:url']");
            var url = new Uri(urlMeta.Attributes["content"].Value, UriKind.Relative);
            var variantId = url.ToString().Split("/").Last();
            var productId = variantId.Contains("-") ? variantId.Substring(0, variantId.IndexOf("-", StringComparison.InvariantCulture)) : variantId;

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