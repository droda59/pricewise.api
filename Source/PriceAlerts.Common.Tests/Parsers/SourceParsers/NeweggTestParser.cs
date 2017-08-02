using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Tests.Parsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    // internal class NeweggTestParser : NeweggParser, ITestParser
    // {
    //     public NeweggTestParser(IRequestClient requestClient)
    //         : base(requestClient)
    //     {
    //     }

    //     public async Task<IEnumerable<Uri>> GetTestProductsUrls()
    //     {
    //         var productUrls = new List<Uri>();

    //         var document = await this.LoadDocument(this.Domain);
            
    //         productUrls.AddRange(
    //             document.GetElementbyId("ShellShocker")
    //                 .SelectSingleNode(".//div[@class='swiper-wrapper]")
    //                 .SelectNodes(".//div[contains(@class, 'item-container')]//a")
    //                 .Select(x => new Uri(x.Attributes["href"].Value)));
            
    //         productUrls.AddRange(
    //             document.GetElementbyId("DailyDeals")
    //                 .SelectSingleNode(".//div[@class='swiper-wrapper]")
    //                 .SelectNodes(".//div[contains(@class, 'item-container')]//a")
    //                 .Select(x => new Uri(x.Attributes["href"].Value)));
            
    //         productUrls.AddRange(
    //             document.GetElementbyId("Spotlight")
    //                 .SelectSingleNode(".//div[@class='swiper-wrapper]")
    //                 .SelectNodes(".//div[contains(@class, 'item-container')]//a")
    //                 .Select(x => new Uri(x.Attributes["href"].Value)));
            
    //         productUrls.AddRange(
    //             document.GetElementbyId("MoreDeals")
    //                 .SelectSingleNode(".//div[@class='swiper-wrapper]")
    //                 .SelectNodes(".//div[contains(@class, 'item-container')]//a")
    //                 .Select(x => new Uri(x.Attributes["href"].Value)));

    //         return productUrls;
    //     }
    // }
}
