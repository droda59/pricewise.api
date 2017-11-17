using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PriceAlerts.Common.Infrastructure
{
    internal class HttpClient : IRequestClient
    {
        private System.Net.Http.HttpClient _httpClient;
        private CookieContainer _cookieContainer;

        public void Initialize()
        {
            this._cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                CookieContainer = this._cookieContainer,
                UseCookies = true
            };

            this._httpClient = new System.Net.Http.HttpClient(handler);
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
            return await this.LoadHtmlAsync(uri, HttpMethod.Get, string.Empty, string.Empty, customHeaders);
        }

        public async Task<HttpResponseMessage> LoadHtmlAsync(Uri uri, HttpMethod httpMethod, string requestBody, string contentType, params KeyValuePair<string, string>[] customHeaders)
        {
            var message = new HttpRequestMessage(httpMethod, uri.AbsoluteUri);

            foreach (var customHeader in customHeaders)
            {
                message.Headers.Add(customHeader.Key, customHeader.Value);
            }

            this._cookieContainer.Add(uri, new Cookie("walmart.csrf", "5f684ab3ad21366b3484c398b2b63f9df34cee1d"));
            this._cookieContainer.Add(uri, new Cookie("walmart.id", "17314a3c-c3fc-418f-8131-4bed70fb5cf8"));

            message.Content = new StringContent(requestBody);

            if(!string.IsNullOrEmpty(contentType))
            {
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }

            var data = await this._httpClient.SendAsync(message);

            return data;
        }
    }
}