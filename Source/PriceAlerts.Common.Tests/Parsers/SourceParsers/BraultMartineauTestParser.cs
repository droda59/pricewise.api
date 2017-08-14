using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Sources;
using PriceAlerts.Common.Tests.Parsers;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class BraultMartineauTestParser : BraultMartineauParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public BraultMartineauTestParser(IDocumentLoader documentLoader)
            : base(documentLoader)
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();

            var pagesToBrowse = new List<Uri>
            {
                new Uri("http://www.braultetmartineau.com/fr/commodes"),
                new Uri("http://www.braultetmartineau.com/fr/televiseurs"),
                new Uri("http://www.braultetmartineau.com/fr/refrigerateurs"),
                new Uri("http://www.braultetmartineau.com/fr/fours"),
                new Uri("http://www.braultetmartineau.com/en/divans"),
                new Uri("http://www.braultetmartineau.com/en/chaises"),
                new Uri("http://www.braultetmartineau.com/en/ensembles-matelas-sommier")
            };

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
                var page = await this._documentLoader.LoadDocument(pageUrl, this.Source.CustomHeaders);
                var productList = page.DocumentNode.SelectSingleNode(".//div[contains(@class, 'product_listing_container')]");
                if (productList != null)
                {
                    lock(lockObject)
                    {
                        productUrls.AddRange(
                            productList
                                .SelectNodes(".//li//a")
                                .Select(x => x.Attributes["href"].Value)
                                .Where(x => x.StartsWith("http"))
                                .Distinct()
                                .Take(4)
                                .Select(x => new Uri(this.Source.Domain, x)));
                    }  
                }
            }));

            return productUrls;
        }
    }
}
