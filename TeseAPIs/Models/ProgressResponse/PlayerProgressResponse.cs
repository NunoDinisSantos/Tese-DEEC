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
        public int Treasure { get; set; }
        public int AncientCoral { get; set; }
        public int LostResearch { get; set; }
        public int TempleJewel { get; set; }
        public int BoatJewel { get; set; }
        public int OldIce { get; set; }
    }
}