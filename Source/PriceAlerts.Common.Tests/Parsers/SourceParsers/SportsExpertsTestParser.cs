using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class SportsExpertsTestParser : SportsExpertsParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public SportsExpertsTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new SportsExpertsSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);

            var urls = new[]
            {
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/en-CA/p-reflect-medium-adult-shoe-spikes/774852/774852-1"),
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-ionic-montre-intelligente-pour-adulte/277436/277436-2"),
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-famous-6xpress-11-w-skis-alpins-de-piste-pour-femme/188047/188047-3"),
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-sw-squad-leader-jr-t-shirt-dentrainement-pour-garcon/125103/125103-1"),
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-buck-jr-lunettes-de-soleil-pour-junior/106042/106042-1"),
            };

            return urls;
        }
    }
}