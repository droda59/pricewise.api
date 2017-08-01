using System;
using System.Linq;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Parsers.SourceParsers
{
    // public class NeweggParser// : BaseParser, IParser
    // {
    //     public NeweggParser(IDocumentLoader documentLoader)
    //         //: base(documentLoader, new NeweggSource())
    //     {
    //     }
        
    //     protected override bool HasRedirectProductUrl(HtmlDocument doc)
    //     {
    //         return doc.GetElementbyId("wrap_shocker") != null;
    //     }

    //     protected override Uri GetRedirectProductUrl(HtmlDocument doc)
    //     {
    //         var redirectUrl = doc
    //             .GetElementbyId("shellShockerViewDetails")
    //             .Attributes["href"].Value;

    //         return new Uri(redirectUrl);
    //     }
        
    //     protected override string GetTitle(HtmlDocument doc)
    //     {
    //         var titleNode = doc.DocumentNode
    //             .SelectSingleNode(".//span[@itemprop='name']");

    //         var extractedValue = titleNode.InnerText.Replace(Environment.NewLine, string.Empty).Trim();

    //         return extractedValue;
    //     }

    //     protected override string GetImageUrl(HtmlDocument doc)
    //     {
    //         var nodeValue = doc.DocumentNode
    //             .SelectSingleNode(".//span[@class='mainSlide']")
    //             .SelectSingleNode(".//img");
                
    //         var extractedValue = nodeValue.Attributes["src"].Value;

    //         return extractedValue;
    //     }

    //     protected override decimal GetPrice(HtmlDocument doc)
    //     {
    //         var priceContent = doc.DocumentNode
    //             .SelectSingleNode(".//div[@itemprop='offers']")
    //             .SelectSingleNode(".//meta[@itemprop='price']")
    //             .Attributes["content"].Value;

    //         var decimalValue = priceContent.ExtractDecimal();

    //         return decimalValue;
    //     }
    // }
}
