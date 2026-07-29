using Licentra.API.Data;
using Licentra.API.Interfaces.Roles;
using Licentra.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Repositories.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly LicentraDbContext _context;

        public RoleRepository(LicentraDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles
                .OrderBy(r => r.RoleName)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int roleId)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        public async Task AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
        }

        public Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Role role)
        {
            _context.Roles.Remove(role);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int roleId)
        {
            return await _context.Roles
                .AnyAsync(r => r.RoleId == roleId);
        }

        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            return await _context.Roles
                .AnyAsync(r => r.RoleName == roleName);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}