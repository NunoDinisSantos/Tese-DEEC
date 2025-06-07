namespace TeseAPIs.Models
{
    public class Challenge
    {
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty;

        public string StartDate {  get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public bool Ended { get; set; } = false;

        public int EventType { get; set; }

        public int QuantityXXX { get; set; }

        public int QuantityYYY { get; set; }

        public int QuantityZZZ { get; set; }
    }
}
