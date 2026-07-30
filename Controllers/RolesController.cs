using Licentra.API.Common.Responses;
using Licentra.API.DTOs.Roles;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleDto>>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<RoleDto>>(
                true,
                "Roles retrieved successfully.",
                roles
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoleDto>>> GetRoleById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
            {
                throw new NotFoundException("Role not found.");
            }

            return Ok(new ApiResponse<RoleDto>(
                true,
                "Role retrieved successfully.",
                role
            ));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole(CreateRoleDto dto)
        {
            var role = await _roleService.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetRoleById),
                new { id = role.RoleId },
                new ApiResponse<RoleDto>(
                    true,
                    "Role created successfully.",
                    role
                ));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateRole(int id, UpdateRoleDto dto)
        {
            var updated = await _roleService.UpdateAsync(id, dto);

            if (!updated)
            {
                throw new NotFoundException("Role not found.");
            }

            return Ok(new ApiResponse<object>(
                true,
                "Role updated successfully.",
                null
            ));
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRole(int id)
        {
            var deleted = await _roleService.DeleteAsync(id);

            if (!deleted)
            {
                throw new NotFoundException("Role not found.");
            }

            return Ok(new ApiResponse<object>(
                true,
                "Role deleted successfully.",
                null
            ));
        }
    }
}   