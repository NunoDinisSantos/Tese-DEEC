namespace TeseAPIs.Models
{
    public class ChallengeProgressData
    {
        public required string PlayerId { get; set; }
        public string Nick_Name { get; set; } = string.Empty;
        public int Coins { get; set; }
        public int FishCaught { get; set; }
        public int Credits { get; set; }
    }
}
