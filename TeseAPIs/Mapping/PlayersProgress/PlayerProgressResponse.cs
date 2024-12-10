namespace TeseAPIs.Mapping.PlayersProgress
{
    public class PlayerProgressResponse
    {
        public required string PlayerId { get; set; }
        public int TempoDeJogo { get; set; }
        public int Moedas { get; set; }
        public int PeixesApanhados { get; set; }
        public int Tutorial { get; set; }
        public int Lanterna { get; set; }
        public int ModuloProfundidade { get; set; }
        public int ModuloTemperatura { get; set; }
        public int ModuloStorage { get; set; }
        public int ModuloReel { get; set; }
        public int Days { get; set; }
        public int ObjectosRaros { get; set; }
        public DateTime LastLogin { get; set; }
        public int DayStreak { get; set; }
        public int Creditos { get; set; }
    }
}