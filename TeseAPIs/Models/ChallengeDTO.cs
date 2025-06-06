namespace TeseAPIs.Models
{
    public class ChallengeDTO
    {
        public string Description { get; set; } = string.Empty;

        public string StartDate { get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public bool Ended { get; set; } = false;
    }
}
