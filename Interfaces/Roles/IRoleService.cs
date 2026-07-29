using Licentra.API.DTOs.Roles;

namespace Licentra.API.Interfaces.Roles
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();

        Task<RoleDto?> GetByIdAsync(int roleId);

        Task<RoleDto> AddAsync(CreateRoleDto dto);

        Task<bool> UpdateAsync(int roleId, UpdateRoleDto dto);

        Task<bool> DeleteAsync(int roleId);
    }
}