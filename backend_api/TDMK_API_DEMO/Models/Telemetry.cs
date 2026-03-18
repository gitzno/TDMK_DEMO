namespace TDMK_API_DEMO.Models
{
    public class Telemetry
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
