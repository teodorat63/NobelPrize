using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NobelPrizeApp
{
    public class NobelPrizesResponse
    {
        [JsonPropertyName("prizes")]
        public List<Prize> Prizes { get; set; }
    }

    public class Prize
    {
        [JsonPropertyName("year")]
        public string Year { get; set; }

        [JsonPropertyName("laureates")]
        public List<Laureate> Laureates { get; set; }
    }

    public class Laureate
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("firstname")]
        public string Firstname { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("motivation")]
        public string Motivation { get; set; }

    }
}
