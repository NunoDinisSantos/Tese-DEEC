namespace TeseAPIs.Models
{
    public class Challenge
    {
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty;

        public string StartDate {  get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public bool Started { get; set; } = false;

        public bool Ended { get; set; } = false;

        public int EventType { get; set; }

        public int QuantityX { get; set; }

        public int QuantityY { get; set; }

        public int QuantityZ { get; set; }
    }
}
