using System;
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
        
        public async Task<ApiTypes.EmailSend> SendEmail(EmailInformation emailInformation)
        {
            emailInformation.Parameters.Add("from", "max@pricewi.se");
            emailInformation.Parameters.Add("apiKey", ApiKey);
            emailInformation.Parameters.Add("to", emailInformation.RecipientAddress);
            emailInformation.Parameters.Add("isTransactional", true.ToString());
            emailInformation.Parameters.Add("template", emailInformation.TemplateName);

            var uriWithQuery = QueryHelpers.AddQueryString("email/send", emailInformation.Parameters);
            var uri = new Uri(uriWithQuery, UriKind.Relative);

            var response = await this._httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonConvert.DeserializeObject<ApiResponse<ApiTypes.EmailSend>>(stringResponse);
            
            return jsonResponse.Data;
        }
    }
}