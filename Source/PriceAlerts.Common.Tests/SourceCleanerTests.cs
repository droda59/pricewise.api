using System;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Cleaners;
using PriceAlerts.Common.Cleaners.Sources;
using PriceAlerts.Common.Tests.Parsers;
using PriceAlerts.Common.Tests.Parsers.SourceParsers;

using Xunit;

namespace PriceAlerts.Common.Tests
{
    public class SourceCleanerTests
    {
        private readonly HtmlLoader _htmlLoader;

        public SourceCleanerTests()
        {
            this._htmlLoader = new HtmlLoader();
        }

        [Theory]
        // [InlineData(typeof(AmazonCleaner), typeof(AmazonTestParser))]
        // [InlineData(typeof(BestBuyCleaner), typeof(BestBuyTestParser))]
        // [InlineData(typeof(CanadianTireCleaner), typeof(CanadianTireTestParser))]
        [InlineData(typeof(IndigoCleaner), typeof(IndigoTestParser))]
        public async Task GetSiteInfo_AlwaysReturnSiteInfo(Type cleanerType, Type parserType)
        {
            var cleaner = this.CreateTestCleaner(cleanerType);
            var parser = this.CreateTestParser(parserType, this._htmlLoader);
            var urlsToTest = (await parser.GetTestProductsUrls()).ToList();

            Assert.NotEmpty(urlsToTest);
            foreach (var urlToTest in urlsToTest)
            {
                try
                {
                    var cleanUrl = cleaner.CleanUrl(urlToTest);
                    Assert.NotNull(cleanUrl);

                    Console.WriteLine($"Cleaning {cleanUrl.AbsoluteUri}");

                    var siteInfo = await parser.GetSiteInfo(cleanUrl);

                    Assert.NotNull(siteInfo);
                    Assert.NotNull(siteInfo.Uri);
                    Assert.NotNull(siteInfo.Title);
                    Assert.NotNull(siteInfo.ImageUrl);
                    Assert.NotNull(siteInfo.ProductIdentifier);
                    Assert.True(siteInfo.Price >= 0);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed test on {urlToTest}");

                    throw e;
                }
            }

            Console.WriteLine($"Ran {urlsToTest.Count} tests on {parser.Domain}");
        }

        private ICleaner CreateTestCleaner(Type parserType)
        {
            return (ICleaner)Activator.CreateInstance(parserType);
        }

        private ITestParser CreateTestParser(Type parserType, IHtmlLoader htmlLoader)
        {
            return (ITestParser)Activator.CreateInstance(parserType, htmlLoader);
        }
    }
}
