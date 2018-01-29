using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace PriceAlerts.Common.Infrastructure
{
    internal class HttpClient : IRequestClient
    {
        private System.Net.Http.HttpClient _httpClient;
        private IDictionary<string, IDictionary<StringSegment, string>> _cookieDick;

        public void Initialize()
        {
            this._cookieDick = new ConcurrentDictionary<string, IDictionary<StringSegment, string>>();
            
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = false
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
            var message = new HttpRequestMessage(new HttpMethod("GET"), uri.AbsoluteUri);

            foreach (var customHeader in customHeaders) 
            {
                message.Headers.Add(customHeader.Key, customHeader.Value);
            }
            
//            var hostCookies = this._cookies.GetCookies(host).Cast<Cookie>().ToList();
//            message.Headers.Add("Cookie", string.Join(';', hostCookies.Select(x => $"{x.Name}={x.Value}").ToList()));

            if (this._cookieDick.ContainsKey(uri.Host))
            {
                CookiesHelper.PutCookiesOnRequest(message, this._cookieDick[uri.Host]);
            }
            
            var data = await this._httpClient.SendAsync(message);


            var responseCookies = CookiesHelper.ExtractCookiesFromResponse(data);

            if (!this._cookieDick.ContainsKey(uri.Host))
            {
                this._cookieDick.Add(uri.Host, responseCookies);
            }

//            var responseCookies = data.Headers.GetValues("Set-Cookie");
//            foreach (var cookie in responseCookies)
//            {
//                var nameValueDelimiter = cookie.IndexOf("=", StringComparison.Ordinal);
//                var cookieName = cookie.Substring(0, nameValueDelimiter);
//                var cookieValue = cookie.Substring(nameValueDelimiter + 1, cookie.Length - 1 - nameValueDelimiter);
//                
//                Console.WriteLine(cookieName + ": " + cookieValue);
//                
//                this._cookies.Add(host, new Cookie(cookie.Key.ToString(), cookie.Value));
//            }
            
            return data;
        }
        
        public class CookiesHelper
        {

            // Inspired from:
            // https://github.com/aspnet/Mvc/blob/538cd9c19121f8d3171cbfddd5d842cbb756df3e/test/Microsoft.AspNet.Mvc.FunctionalTests/TempDataTest.cs#L201-L202

            public static IDictionary<StringSegment, string> ExtractCookiesFromResponse(HttpResponseMessage response)
            {
                var result = new Dictionary<StringSegment, string>();
                if (response.Headers.TryGetValues("Set-Cookie", out var values))
                {
                    SetCookieHeaderValue.ParseList(values.ToList()).ToList().ForEach(cookie =>
                    {
                        result.Add(cookie.Name, cookie.Value.ToString());
                    });
                }
                
                return result;
            }

            public static HttpRequestMessage PutCookiesOnRequest(HttpRequestMessage request, IDictionary<StringSegment, string> cookies)
            {
                cookies.Keys.ToList().ForEach(key =>
                {
                    request.Headers.Add("Cookie", new CookieHeaderValue(key, cookies[key]).ToString());
                });

                return request;
            }
        }
    }
}