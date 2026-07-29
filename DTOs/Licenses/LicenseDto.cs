namespace Licentra.API.DTOs.Licenses
{
    public class LicenseDto
    {
        public int LicenseId { get; set; }

        public int SoftwareId { get; set; }

        public string SoftwareName { get; set; } = string.Empty;

        public string LicenseKey { get; set; } = string.Empty;

        public string LicenseType { get; set; } = string.Empty;

        public DateOnly PurchaseDate { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public decimal PurchaseCost { get; set; }

        public int Seats { get; set; }

        public byte LicenseStatus { get; set; }

        public string? Notes { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}