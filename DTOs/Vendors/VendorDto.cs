namespace Licentra.API.DTOs.Vendors
{
    public class VendorDto
    {
        public int VendorId { get; set; }

        public string VendorName { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Website { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}