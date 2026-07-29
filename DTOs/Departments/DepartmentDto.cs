namespace Licentra.API.DTOs.Departments
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}