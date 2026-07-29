namespace Licentra.API.DTOs.LicenseAssignments
{
    public class LicenseAssignmentDto
    {
        public int AssignmentId { get; set; }

        public int LicenseId { get; set; }

        public string LicenseKey { get; set; } = string.Empty;

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public byte AssignmentStatus { get; set; }

        public int AssignedByUserId { get; set; }

        public string AssignedByUsername { get; set; } = string.Empty;

        public string? Remarks { get; set; }
    }
}