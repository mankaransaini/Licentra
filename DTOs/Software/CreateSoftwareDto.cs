namespace Licentra.API.DTOs.Software
{
    public class CreateSoftwareDto
    {
        public int VendorId { get; set; }

        public string SoftwareName { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? Description { get; set; }

        public bool IsSubscription { get; set; }
    }
}