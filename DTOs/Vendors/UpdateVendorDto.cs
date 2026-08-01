namespace Licentra.API.DTOs.Vendors
{
    public class UpdateVendorDto
    {
        public string VendorName { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Website { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; }
    }
}