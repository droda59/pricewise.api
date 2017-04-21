using Newtonsoft.Json;

namespace PriceAlerts.Common.Models
{
    public abstract class Document
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
    }
}