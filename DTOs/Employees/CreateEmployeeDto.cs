namespace Licentra.API.DTOs.Employees
{
    public class CreateEmployeeDto
    {
        public string EmployeeCode { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public int DepartmentId { get; set; }
        public string Designation { get; set; } = string.Empty;

        public DateOnly JoiningDate { get; set; }

        public string? EmploymentStatus { get; set; }
    }
}