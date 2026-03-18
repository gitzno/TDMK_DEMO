namespace TDMK_API_DEMO.DTOs
{
    public class DTOTelemetry
    {
        public double Value { get; set; } // Số liệu line track

        public IFormFile? Image { get; set; } // File ảnh từ Multipart Form

        public string DeviceId { get; set; } = "Unknown";
    }
}
