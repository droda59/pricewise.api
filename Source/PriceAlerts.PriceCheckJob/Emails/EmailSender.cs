using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.WebUtilities;

using Newtonsoft.Json;

using PriceAlerts.PriceCheckJob.Models;

namespace PriceAlerts.PriceCheckJob.Emails
{
    internal class EmailSender : IEmailSender
    {
        private static string ApiKey = "da24aadb-b4b4-48d4-9f87-9c414ccc10e0";

        private static readonly string ApiUri = "https://api.elasticemail.com/v2/";

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
        
        public async Task<ApiTypes.EmailSend> SendEmail(PriceChangeAlert alert)
        {
            var parametersToAdd = new Dictionary<string, string> 
            { 
                { "apiKey", ApiKey },
                { "to", alert.EmailAddress },
                { "merge_firstname" , alert.FirstName },
                { "merge_productName" , alert.AlertTitle },
                { "merge_previousPrice" , alert.PreviousPrice.ToString(CultureInfo.InvariantCulture) },
                { "merge_newPrice" , alert.NewPrice.ToString(CultureInfo.InvariantCulture) },
                { "merge_productUrl" , alert.ProductUri.AbsoluteUri },
                { "merge_productDomain", alert.ProductUri.Authority },
                { "merge_imageUrl", alert.ImageUrl }, 
                { "isTransactional", true.ToString() },
                { "template", "Price change" }
            };

            var uriWithQuery = QueryHelpers.AddQueryString("email/send", parametersToAdd);
            var uri = new Uri(uriWithQuery, UriKind.Relative);

            var response = await this._httpClient.PostAsync(uri, new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonConvert.DeserializeObject<ApiResponse<ApiTypes.EmailSend>>(stringResponse);
            
            return jsonResponse.Data;
        }
    }
}