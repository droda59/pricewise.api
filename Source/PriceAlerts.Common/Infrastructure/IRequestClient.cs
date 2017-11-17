using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriceAlerts.Common.Infrastructure
{
    public interface IRequestClient : IDisposable
    {
        Task<string> ReadHtmlAsync(Uri uri, params KeyValuePair<string, string>[] customHeaders);

        Task<HttpResponseMessage> LoadHtmlAsync(Uri uri, params KeyValuePair<string, string>[] customHeaders);

        Task<HttpResponseMessage> LoadHtmlAsync(Uri uri, HttpMethod httpMethod, string requestBody, string contentType, params KeyValuePair<string, string>[] customHeaders);

        void Initialize();
    }
}
