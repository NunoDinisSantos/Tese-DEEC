using System.Text.Json.Serialization;

namespace SuperUser.Models
{
    public class Estudante
    {
        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }

        [JsonPropertyName("tempoDeJogo")]
        public int TempoDeJogo { get; set; }

        [JsonPropertyName("moedas")]
        public int Moedas { get; set; }

        [JsonPropertyName("peixesApanhados")]
        public int PeixesApanhados { get; set; }

        [JsonPropertyName("tutorial")]
        public int Tutorial { get; set; }

        [JsonPropertyName("lanterna")]
        public int Lanterna { get; set; }

        [JsonPropertyName("moduloProfundidade")]
        public int ModuloProfundidade { get; set; }

        [JsonPropertyName("moduloTemperatura")]
        public int ModuloTemperatura { get; set; }

        [JsonPropertyName("moduloStorage")]
        public int ModuloStorage { get; set; }

        [JsonPropertyName("moduloReel")]
        public int ModuloReel { get; set; }

        [JsonPropertyName("days")]
        public int Days { get; set; }

        [JsonPropertyName("objectosRaros")]
        public int ObjectosRaros { get; set; }

        [JsonPropertyName("lastLogin")]
        public DateTime LastLogin { get; set; }

        [JsonPropertyName("dayStreak")]
        public int DayStreak { get; set; }

        [JsonPropertyName("creditos")]
        public int Creditos { get; set; }
    }
}