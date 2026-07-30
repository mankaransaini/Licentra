using Licentra.API.Common.Responses;
using Licentra.API.DTOs.Users;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<UserDto>>(
                true,
                "Users retrieved successfully.",
                users
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                throw new NotFoundException("User not found.");

            return Ok(new ApiResponse<UserDto>(
                true,
                "User retrieved successfully.",
                user
            ));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(CreateUserDto dto)
        {
            var user = await _userService.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = user.UserId },
                new ApiResponse<UserDto>(
                    true,
                    "User created successfully.",
                    user
                ));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUser(int id, UpdateUserDto dto)
        {
            var updated = await _userService.UpdateAsync(id, dto);

            if (!updated)
                throw new NotFoundException("User not found.");

            return Ok(new ApiResponse<object>(
                true,
                "User updated successfully.",
                null
            ));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteAsync(id);

            if (!deleted)
                throw new NotFoundException("User not found.");

            return Ok(new ApiResponse<object>(
                true,
                "User deleted successfully.",
                null
            ));
        }
    }
}