namespace Licentra.API.DTOs.Software
{
    public class SoftwareDto
    {
        public int SoftwareId { get; set; }

        public int VendorId { get; set; }

        public string VendorName { get; set; } = string.Empty;

        public string SoftwareName { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? Description { get; set; }

        public bool IsSubscription { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}