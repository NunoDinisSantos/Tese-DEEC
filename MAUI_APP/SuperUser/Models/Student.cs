using System.Text.Json.Serialization;

namespace SuperUser.Models
{
    public class Student
    {
        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }

        [JsonPropertyName("timePlayed")]
        public int TimePlayed { get; set; }

        [JsonPropertyName("coins")]
        public int Coins { get; set; }

        [JsonPropertyName("fishCaught")]
        public int FishCaught { get; set; }

        [JsonPropertyName("tutorial")]
        public int Tutorial { get; set; }

        [JsonPropertyName("flashlight")]
        public int Flashlight { get; set; }

        [JsonPropertyName("depthModule")]
        public int DepthModule { get; set; }

        [JsonPropertyName("tempModule")]
        public int TempModule { get; set; }

        [JsonPropertyName("storageModule")]
        public int StorageModule { get; set; }

        [JsonPropertyName("reelModule")]
        public int ReelModule { get; set; }

        [JsonPropertyName("days")]
        public int Days { get; set; }

        [JsonPropertyName("rareObjects")]
        public int RareObjects { get; set; }

        [JsonPropertyName("lastLogin")]
        public DateTime LastLogin { get; set; }

        [JsonPropertyName("dayStreak")]
        public int DayStreak { get; set; }

        [JsonPropertyName("credits")]
        public int Credits { get; set; }

        [JsonPropertyName("treasure")]
        public int Treasure { get; set; }

        [JsonPropertyName("lost_research")]
        public int LostResearch { get; set; }

        [JsonPropertyName("boat_jewel")]
        public int BoatJewel { get; set; }

        [JsonPropertyName("temple_jewel")]
        public int TempleJewel { get; set; }

        [JsonPropertyName("ancient_coral")]
        public int AncientCoral { get; set; }

        [JsonPropertyName("old_ice")]
        public int OldIce { get; set; }
    }
}