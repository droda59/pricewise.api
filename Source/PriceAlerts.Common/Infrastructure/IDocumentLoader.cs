using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Infrastructure
{
    public interface IDocumentLoader
    {
        Task<HtmlDocument> LoadDocument(Uri url, IEnumerable<KeyValuePair<string, string>> customHeaders);

        Task<string> LoadDocumentAsString(Uri url, HttpMethod httpMethod, string requestBody, string contentType, IEnumerable<KeyValuePair<string, string>> customHeaders);
    }
}