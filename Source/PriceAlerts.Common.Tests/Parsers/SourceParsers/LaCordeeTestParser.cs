using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class LaCordeeTestParser : LaCordeeParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public LaCordeeTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new LaCordeeSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);

            var urls = new[]
            {
                new Uri(this.Source.Domain, "https://www.lacordee.com/en/the-north-face-m-s-dryzzle-jacket-406025"),
                new Uri(this.Source.Domain, "https://www.lacordee.com/en/wintersports/skate-cross-country-skiing/rossignol-m-s-x-8-skate-cross-country-boots"),
                new Uri(this.Source.Domain, "https://www.lacordee.com/en/wintersports/skate-cross-country-skiing/rossignol-delta-skating-ifp-cross-country-skis"),
                new Uri(this.Source.Domain, "https://www.lacordee.com/en/wintersports/skate-cross-country-skiing/louis-garneau-nordic-shield-ski-goggles"),
                new Uri(this.Source.Domain, "https://www.lacordee.com/en/climbing/climbing-protection/fixe-10mm-3-8-bolt-hanger"),
                new Uri(this.Source.Domain, "https://www.lacordee.com/fr/soldes/sports-d-hiver/fixations-de-planche-a-neige-divisible-spark-surge-femmes"),
            };

            return urls;
        }
    }
}