using Licentra.API.Models;

namespace Licentra.API.Interfaces.Roles
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();

        Task<Role?> GetByIdAsync(int roleId);

        Task<Role?> GetByNameAsync(string roleName);

        Task AddAsync(Role role);

        Task UpdateAsync(Role role);

        Task DeleteAsync(Role role);

        Task<bool> ExistsAsync(int roleId);

        Task<bool> RoleNameExistsAsync(string roleName);

        Task SaveChangesAsync();
    }
}