namespace TeseAPIs.Mapping.PlayersProgress
{
    public class PlayerProgressResponse
    {
        public required string PlayerId { get; set; }
        public int TimePlayed { get; set; }
        public int Coins { get; set; }
        public int FishCaught { get; set; }
        public int Tutorial { get; set; }
        public int Flashlight { get; set; }
        public int DepthModule { get; set; }
        public int TempModule { get; set; }
        public int StorageModule { get; set; }
        public int ReelModule { get; set; }
        public int Days { get; set; }
        public int RareObjects { get; set; }
        public DateTime LastLogin { get; set; }
        public int DayStreak { get; set; }
        public int Credits { get; set; }
        public bool Treasure { get; set; }
        public bool AncientCoral { get; set; }
        public bool LostResearch { get; set; }
        public bool TempleJewel { get; set; }
        public bool BoatJewel { get; set; }
        public bool OldIce { get; set; }
    }
}