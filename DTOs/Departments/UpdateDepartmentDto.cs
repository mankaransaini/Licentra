using System.ComponentModel.DataAnnotations;

namespace Licentra.API.DTOs.Departments
{
    public class UpdateDepartmentDto
    {
        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}