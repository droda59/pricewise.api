using System;
using System.Collections.Generic;
using System.Linq;

using HtmlAgilityPack;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    public class SearsParser : BaseParser, IParser
    {
        public SearsParser(IDocumentLoader documentLoader)
            : base(documentLoader, new SearsSource())
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
                var startOfJavascript = mediaViewerScriptNode.InnerText.IndexOf("JSON.parse('");
                var endOfJavascript = mediaViewerScriptNode.InnerText.IndexOf("');");

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
            var priceNode = doc.GetElementbyId("price-sales-container").SelectSingleNode(".//p[contains(@class, 'price-sales')]");

            var decimalValue = priceNode.InnerText.ExtractDecimal();

            return decimalValue;
        }
    }
}
