using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriceAlerts.Common
{
    internal class HtmlLoader : IHtmlLoader
    {
        private HttpClient _httpClient;

        public void Initialize()
        {
            this._httpClient = new HttpClient();
        }

        public void Dispose()
        {
            this._httpClient.Dispose();
            this._httpClient = null;
        }

        public async Task<string> ReadHtmlAsync(Uri uri, params KeyValuePair<string, string>[] customHeaders)
        {
            var message = new HttpRequestMessage(new HttpMethod("GET"), uri.AbsoluteUri);

            foreach (var customHeader in customHeaders) 
            {
                message.Headers.Add(customHeader.Key, customHeader.Value);
            }

            string content = null;
            var data = await this._httpClient.SendAsync(message);
            if (data.IsSuccessStatusCode)
            {
                content = await data.Content.ReadAsStringAsync();
            }
            
            return content;
        }
    }
}