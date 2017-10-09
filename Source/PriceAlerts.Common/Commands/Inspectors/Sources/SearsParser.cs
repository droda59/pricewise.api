using System;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PriceAlerts.Common.Extensions;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors.Sources
{
    public class SearsParser : BaseHtmlParser
    {
        public SearsParser(IDocumentLoader documentLoader, SearsSource source)
            : base(documentLoader, source)
        {
        }

        protected override string GetTitle(HtmlDocument doc)
        {
            var titleNode = doc.GetElementbyId("pdpMain").SelectSingleNode("//h1");

            var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

            return extractedValue;
        }

        protected override string GetImageUrl(HtmlDocument doc)
        {
            var mediaViewerScriptNode = doc.GetElementbyId("pdpMain")
                .SelectSingleNode(".//div[contains(@class, 'product-image-container')]")
                .SelectSingleNode(".//script[@type='text/javascript' and not(@src)]");

            if (mediaViewerScriptNode != null)
            {
                var startOfJavascript = mediaViewerScriptNode.InnerText.IndexOf("JSON.parse('", StringComparison.InvariantCulture);
                var endOfJavascript = mediaViewerScriptNode.InnerText.IndexOf("');", StringComparison.InvariantCulture);

                var json = mediaViewerScriptNode.InnerText.Substring(startOfJavascript + 12, endOfJavascript - startOfJavascript - 12);
                dynamic jsonResult = JsonConvert.DeserializeObject(json);
                if (jsonResult.set != null)
                {
                    var name = ((JArray)jsonResult.set).First()["name"];

                    return new Uri($"http://i1.adis.ws/i/searsca/{name}").AbsoluteUri;
                }
            }

            return string.Empty;
        }

        protected override decimal GetPrice(HtmlDocument doc)
        {
            var priceNode = doc.GetElementbyId("price-sales-container").SelectSingleNode(".//*[contains(@class, 'price-sales')]");

            var decimalValue = priceNode.InnerText.ExtractDecimal();

            return decimalValue;
        }
    }
}
