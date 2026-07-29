using Licentra.API.DTOs.Roles;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.Roles;
using Licentra.API.Models;

namespace Licentra.API.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository ??
                throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            return roles.Select(r => new RoleDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                Description = r.Description,
                IsActive = r.IsActive ?? true
            });
        }

        public async Task<RoleDto?> GetByIdAsync(int roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);

            if (role == null)
                return null;

            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive ?? true
            };
        }

        public async Task<RoleDto> AddAsync(CreateRoleDto dto)
        {
            if (await _roleRepository.RoleNameExistsAsync(dto.RoleName))
            {
                throw new ConflictException("Role name already exists.");
            }

            var role = new Role
            {
                RoleName = dto.RoleName.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = true
            };

            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();

            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Description = role.Description,
                IsActive = role.IsActive ?? true
            };
        }

        public async Task<bool> UpdateAsync(int roleId, UpdateRoleDto dto)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);

            if (role == null)
                return false;

            if (!string.Equals(role.RoleName, dto.RoleName, StringComparison.OrdinalIgnoreCase)
                && await _roleRepository.RoleNameExistsAsync(dto.RoleName))
            {
                throw new ConflictException("Role name already exists.");
            }

            role.RoleName = dto.RoleName.Trim();
            role.Description = dto.Description?.Trim();
            role.IsActive = dto.IsActive;

            await _roleRepository.UpdateAsync(role);
            await _roleRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);

            if (role == null)
                return false;

            await _roleRepository.DeleteAsync(role);
            await _roleRepository.SaveChangesAsync();

            return true;
        }
    }
}