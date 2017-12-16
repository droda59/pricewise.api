using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class MECTestParser : MECParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public MECTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new MECSource())
        {
            this._documentLoader = documentLoader;
        }

        public Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var urls = new[]
            {
                new Uri(this.Source.Domain, "https://www.mec.ca/en/product/5047-695/Tremblant-Lite-Down-Jacket"),
                new Uri(this.Source.Domain, "https://www.mec.ca/en/product/5053-841/%5Bak%5D-BK-Lite-Insulator-Jacket"),
                new Uri(this.Source.Domain, "https://www.mec.ca/en/product/5058-425/Pro-Wood-Fired-Pizza-Oven"),
                new Uri(this.Source.Domain, "https://www.mec.ca/en/product/5056-346/Lector-4-9-LC-Bicycle"),
                new Uri(this.Source.Domain, "https://www.mec.ca/en/product/5036-093/Outpost-Pack"),
            };

            return Task.FromResult(urls.AsEnumerable());
        }
    }
}