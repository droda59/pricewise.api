using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class IkeaTestParser : IkeaParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public IkeaTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new IkeaSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var lockObject = new object();
            var productUrls = new List<Uri>();
            var pagesToBrowse = new List<Uri>();

            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);
            
            var urls = document.GetElementbyId("menu")
                .SelectSingleNode(".//div[contains(@class, 'mainMenu')]")
                .SelectSingleNode(".//div[contains(@class, 'tableContainer')]")
                .SelectNodes(".//tr//td//a[contains(@href, 'catalog/categories')]")
                .Select(x => x.Attributes["href"].Value);

            foreach (var item in urls)
            {
                var queryStringStart = item.IndexOf('?');
                var url = queryStringStart > -1 ? item.Substring(0, item.IndexOf('?')) : item;
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    pagesToBrowse.Add(new Uri(url));
                }
                else
                {
                    pagesToBrowse.Add(new Uri(this.Source.Domain, url));
                }
            }

            await Task.WhenAll(pagesToBrowse.Select(async pageUrl => 
            {
                var page = await this._documentLoader.LoadDocument(pageUrl, this.Source.CustomHeaders);

                var productSelectorNode = page.GetElementbyId("bestseller-selector");
                if (productSelectorNode != null)
                {
                    string script = null;
                    var nextSibling = productSelectorNode.NextSibling;
                    while (nextSibling != null )
                    {
                        if (nextSibling.NodeType == HtmlNodeType.Element && nextSibling.Name == "script" && nextSibling.Attributes["src"] == null)
                        {
                            script = nextSibling.InnerText;
                            break;
                        }

                        nextSibling = nextSibling.NextSibling;
                    }

                    if (script != null)
                    {
                        var startOfJavascript = script.IndexOf("bestsellerTool(", StringComparison.Ordinal);
                        var lengthOfStart = "bestsellerTool(".Length;
                        var endOfJavascript = script.LastIndexOf("}", StringComparison.Ordinal) + 1;

                        var json = script.Substring(startOfJavascript + lengthOfStart, endOfJavascript - startOfJavascript - lengthOfStart);
                        dynamic jsonResult = JsonConvert.DeserializeObject(json);
                        if (jsonResult.data != null)
                        {
                            foreach (var item in ((JArray)jsonResult.data))
                            {
                                lock(lockObject)
                                {
                                    productUrls.Add(new Uri(this.Source.Domain, item["link"].ToString() + "/"));
                                }
                            }
                        }
                    }
                }  
            }));

            return productUrls;
        }
    }
}
