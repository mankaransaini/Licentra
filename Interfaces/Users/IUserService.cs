using Licentra.API.DTOs.Users;

namespace Licentra.API.Interfaces.Users
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();

        Task<UserDto?> GetByIdAsync(int userId);

        Task<UserDto> AddAsync(CreateUserDto dto);

        Task<bool> UpdateAsync(int userId, UpdateUserDto dto);

        Task<bool> DeleteAsync(int userId);
    }
}