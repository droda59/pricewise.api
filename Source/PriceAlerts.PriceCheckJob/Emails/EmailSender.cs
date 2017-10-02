using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.WebUtilities;

using Newtonsoft.Json;

namespace PriceAlerts.PriceCheckJob.Emails
{
    internal class EmailSender : IEmailSender
    {
        private const string ApiKey = "da24aadb-b4b4-48d4-9f87-9c414ccc10e0";
        private const string ApiUri = "https://api.elasticemail.com/v2/";

        private HttpClient _httpClient;

        public void Initialize()
        {
            this._httpClient = new HttpClient();
            this._httpClient.BaseAddress = new Uri(ApiUri);
        }

        public void Dispose()
        {
            this._httpClient.Dispose();
            this._httpClient = null;
        }
        
        public async Task<ApiTypes.EmailSend> SendEmail(string emailAddress, IDictionary<string, string> parameters, string templateName)
        {
            parameters.Add("apiKey", ApiKey);
            parameters.Add("to", emailAddress);
            parameters.Add("isTransactional", true.ToString());
            parameters.Add("template", templateName);

            var uriWithQuery = QueryHelpers.AddQueryString("email/send", parameters);
            var uri = new Uri(uriWithQuery, UriKind.Relative);

            var response = await this._httpClient.PostAsync(uri, new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonConvert.DeserializeObject<ApiResponse<ApiTypes.EmailSend>>(stringResponse);
            
            return jsonResponse.Data;
        }
    }
}