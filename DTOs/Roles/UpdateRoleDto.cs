namespace Licentra.API.DTOs.Roles
{
    public class UpdateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}