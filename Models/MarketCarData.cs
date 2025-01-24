using System.Text.Json.Serialization;

namespace Melbon_Car_Sale_Management_System.Models
{
    public class MarketCarData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class CarApiResponse
    {
        [JsonPropertyName("collection")]
        public CollectionInfo Collection { get; set; }

        [JsonPropertyName("data")]
        public List<MarketCarData> Data { get; set; }
    }

    public class CollectionInfo
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }
    }
} 