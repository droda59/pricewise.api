using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriceAlerts.Common
{
    internal class HtmlLoader : IHtmlLoader
    {
        private HttpClient _httpClient;

        public void Initialize()
        {
            var handler = new HttpClientHandler() 
            {
                AllowAutoRedirect = false
            };
            
            this._httpClient = new HttpClient(handler);
        }

        public void Dispose()
        {
            this._httpClient.Dispose();
            this._httpClient = null;
        }


        public async Task<string> ReadHtmlAsync(Uri uri, params KeyValuePair<string, string>[] customHeaders)
        {
            string content = null;

            var data = await this.LoadHtmlAsync(uri, customHeaders);
            if (data.IsSuccessStatusCode)
            {
                content = await data.Content.ReadAsStringAsync();
            }
            
            return content;
        }

        public async Task<HttpResponseMessage> LoadHtmlAsync(Uri uri, params KeyValuePair<string, string>[] customHeaders)
        {
            var message = new HttpRequestMessage(new HttpMethod("GET"), uri.AbsoluteUri);

            foreach (var customHeader in customHeaders) 
            {
                message.Headers.Add(customHeader.Key, customHeader.Value);
            }

            var data = await this._httpClient.SendAsync(message);
            
            return data;
        }
    }
}