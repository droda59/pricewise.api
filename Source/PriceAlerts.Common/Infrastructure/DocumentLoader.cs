using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Infrastructure
{
    internal class DocumentLoader : IDocumentLoader, IDisposable
    {
        private readonly IRequestClient _requestClient;

        public DocumentLoader(IRequestClient requestClient)
        {
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

        public async Task<string> LoadDocumentAsString(Uri url, HttpMethod httpMethod, string requestBody, string contentType, IEnumerable<KeyValuePair<string, string>> customHeaders)
        {
            var data = await this._requestClient.LoadHtmlAsync(url, httpMethod, requestBody, contentType, customHeaders.ToArray());
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

                    return await this.LoadDocumentAsString(location, httpMethod, requestBody, contentType, customHeaders);
                }
            }
            else
            {
                var content = await data.Content.ReadAsStringAsync();
                return content;
            }

            return null;
        }
    }
}