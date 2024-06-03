using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NobelPrizeApp
{
    public class NobelPrizesResponse
    {
        [JsonPropertyName("nobelPrizes")]
        public List<Prize> Prizes { get; set; }
    }

    public class Prize
    {
        [JsonPropertyName("awardYear")]
        public string Year { get; set; }

        [JsonPropertyName("dateAwarded")]
        public string DateAwarded { get; set; }

        [JsonPropertyName("laureates")]
        public List<Laureate> Laureates { get; set; }
    }

    public class Laureate
    {
        [JsonPropertyName("knownName")]
        public KnownName KnownName { get; set; }

        [JsonPropertyName("motivation")]
        public Motivation Motivation { get; set; }

    }

    public class KnownName
    {
        [JsonPropertyName("en")]
        public string EnglishName { get; set; }

    }

    public class Motivation
    {
        [JsonPropertyName("en")]
        public string EnglishMotivation { get; set; }
    }
}
