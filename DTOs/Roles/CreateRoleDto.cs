namespace Licentra.API.DTOs.Roles
{
    public class CreateRoleDto
    {
        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}