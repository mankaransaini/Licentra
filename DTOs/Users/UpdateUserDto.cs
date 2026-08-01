namespace Licentra.API.DTOs.Users
{
    public class UpdateUserDto
    {
        public int RoleId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}