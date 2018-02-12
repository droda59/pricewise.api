using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;

using Microsoft.Extensions.Logging;

namespace PriceAlerts.Common.Infrastructure
{
    internal class DocumentLoader : IDocumentLoader, IDisposable
    {
        private readonly IRequestClient _requestClient;
        private readonly ILogger _logger;

        public DocumentLoader(IRequestClient requestClient, ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger("DocumentLoader");
            
            this._requestClient = requestClient;
            if (this._requestClient != null)
            {
                this._requestClient.Initialize();
            }
        }

        public void Dispose()
        {
            this._requestClient.Dispose();
        }

        public async Task<HtmlDocument> LoadDocument(Uri url, IEnumerable<KeyValuePair<string, string>> customHeaders)
        {
            var data = await this._requestClient.LoadHtmlAsync(url, customHeaders.ToArray());
            if (!data.IsSuccessStatusCode)
            {
                if (data.Headers.Location != null)
                {
                    Uri location;
                    if (Uri.IsWellFormedUriString(data.Headers.Location.ToString(), UriKind.Absolute))
                    {
                        location = data.Headers.Location;
                    }
                    else
                    {
                        var domain = new Uri(url.GetComponents(UriComponents.Scheme | UriComponents.StrongAuthority, UriFormat.Unescaped));
                        location = new Uri(domain, data.Headers.Location);
                    }

                    return await this.LoadDocument(location, customHeaders);
                }

                if (data.StatusCode == HttpStatusCode.Forbidden)
                {
                    this._logger.LogError("Received a forbidden while trying to retrieve the page. The request is most likely missing a user-agent in it's header.", new HttpRequestException());
                }
            }
            else
            {
                var content = await data.Content.ReadAsStringAsync();

                var document = new HtmlDocument();
                document.LoadHtml(WebUtility.HtmlDecode(content));

                return document;
            }

            return null;
        }
    }
}