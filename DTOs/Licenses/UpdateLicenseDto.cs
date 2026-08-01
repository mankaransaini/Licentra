namespace Licentra.API.DTOs.Licenses
{
    public class UpdateLicenseDto
    {
        public int SoftwareId { get; set; }

        public string LicenseKey { get; set; } = string.Empty;

        public string LicenseType { get; set; } = string.Empty;

        public DateOnly PurchaseDate { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public decimal PurchaseCost { get; set; }

        public int Seats { get; set; }

        public byte LicenseStatus { get; set; }

        public string? Notes { get; set; }
    }
}