using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Tests.Parsers;
using PriceAlerts.Common.Tests.Parsers.SourceParsers;

using Xunit;

namespace PriceAlerts.Common.Tests
{
    public class SourceParserTests
    {
        private readonly HtmlLoader _htmlLoader;

        public SourceParserTests()
        {
            this._htmlLoader = new HtmlLoader();
        }

        [Theory]
        [InlineData(typeof(AmazonTestParser))]
        [InlineData(typeof(ArchambaultTestParser))]
        [InlineData(typeof(BestBuyTestParser))]
        [InlineData(typeof(CanadianTireTestParser))]
        [InlineData(typeof(LegoTestParser))]
        // [InlineData(typeof(NeweggTestParser))]
        [InlineData(typeof(RenaudBrayTestParser))]
        [InlineData(typeof(StaplesTestParser))]
        [InlineData(typeof(TigerDirectTestParser))]
        [InlineData(typeof(ToysRUsTestParser))]
        [InlineData(typeof(IndigoTestParser))]
        public async Task GetSiteInfo_AlwaysReturnSiteInfo(Type parserType)
        {
            var parser = this.CreateTestParser(parserType, this._htmlLoader);
            var urlsToTest = (await parser.GetTestProductsUrls()).ToList();

            Assert.NotEmpty(urlsToTest);
            foreach (var urlToTest in urlsToTest)
            {
                // Console.WriteLine($"Testing {urlToTest.AbsoluteUri}");

                var siteInfo = await parser.GetSiteInfo(urlToTest);

                Assert.NotNull(siteInfo);
                Assert.NotNull(siteInfo.Uri);
                Assert.NotNull(siteInfo.Title);
                Assert.NotNull(siteInfo.ImageUrl);
                Assert.True(siteInfo.Price >= 0);
            }

            Console.WriteLine($"Ran {urlsToTest.Count} tests on {parser.Domain}");
        }

        private ITestParser CreateTestParser(Type parserType, IHtmlLoader htmlLoader)
        {
            return (ITestParser)Activator.CreateInstance(parserType, htmlLoader);
        }
    }
}
