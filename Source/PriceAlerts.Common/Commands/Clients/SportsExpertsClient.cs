using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PriceAlerts.Common.Commands.Clients
{
    public class SportsExpertsClient : IDisposable
    {
        private readonly Uri _apiUrl;
        private readonly IEnumerable<KeyValuePair<string, string>> _customHeaders;

        private HttpClient _httpClient;

        public SportsExpertsClient()
        {
            this._apiUrl = new Uri("https://www.sportsexperts.ca/api/product/calculatePrices");
            this._customHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36"),
                new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"),
                new KeyValuePair<string, string>("Accept", "*/*"),
                new KeyValuePair<string, string>("Accept-Encoding", "gzip, deflate, br"),
                new KeyValuePair<string, string>("Accept-Language", "en-US,en;q=0.9,fr;q=0.8"),
            };

            var handler = new HttpClientHandler
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

        public async Task<JObject> GetPrices(ProductList products)
        {
            var data = await this.PostJsonAsync(this._apiUrl, JsonConvert.SerializeObject(products), this._customHeaders.ToArray());

            var content = await data.Content.ReadAsStringAsync();

            return JObject.Parse(content);
        }

        public async Task<HttpResponseMessage> PostJsonAsync(Uri uri, string requestBody, params KeyValuePair<string, string>[] customHeaders)
        {
            var message = new StringContent(requestBody, Encoding.UTF8, "application/json");

            foreach (var customHeader in customHeaders)
            {
                this._httpClient.DefaultRequestHeaders.Add(customHeader.Key, customHeader.Value);
            }

            return await this._httpClient.PostAsync(uri.AbsoluteUri, message);
        }
    }

    public sealed class ProductList
    {
        public ProductList(IList<string> productIds)
        {
            this.Products = productIds;
        }

        public IList<string> Products { get; set; }
    }
}