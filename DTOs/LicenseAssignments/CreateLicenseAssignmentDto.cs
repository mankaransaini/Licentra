namespace Licentra.API.DTOs.LicenseAssignments
{
    public class CreateLicenseAssignmentDto
    {
        public int LicenseId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? ReturnedDate { get; set; }

        public byte AssignmentStatus { get; set; }

        public int AssignedByUserId { get; set; }

        public string? Remarks { get; set; }
    }
}