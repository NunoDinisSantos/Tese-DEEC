namespace TeseAPIs.Models
{
    public class ChallengeWinnerData
    {
        public required int Id { get; set; }
        public string Player_Id { get; set; } = string.Empty;
        public int ChallengeId { get; set; }
        public string Nick_Name { get; set; } = string.Empty;
    }
}
