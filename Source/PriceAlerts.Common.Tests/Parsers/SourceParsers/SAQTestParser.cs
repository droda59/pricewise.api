using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class SAQTestParser : SAQParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public SAQTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new SAQSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(new Uri(this.Source.Domain, "http://www.saq.com/webapp/wcs/stores/servlet/SearchDisplay?pageSize=20&searchTerm=*&catalogId=50000&showOnly=product&beginIndex=0&langId=-2&storeId=20002&categoryIdentifier=06&orderBy=1"), this.Source.CustomHeaders);

            return document.DocumentNode
                    .SelectNodes("//p[@class='nom'] //a")
                    .Select(x => x.Attributes["href"].Value)
                    .Distinct()
                    .Select(x => new Uri(x));
        }
    }
}
