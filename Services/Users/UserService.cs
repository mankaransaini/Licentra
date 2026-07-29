using Licentra.API.DTOs.Users;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.Users;
using Licentra.API.Models;
using Licentra.API.Interfaces.Security;

namespace Licentra.API.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ??
                throw new ArgumentNullException(nameof(userRepository));

            _passwordHasher = passwordHasher ??
                throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserDto
            {
                UserId = u.UserId,
                EmployeeId = u.EmployeeId,
                EmployeeName = $"{u.Employee.FirstName} {u.Employee.LastName}",
                RoleId = u.RoleId,
                RoleName = u.Role.RoleName,
                Username = u.Username,
                Email = u.Email,
                LastLogin = u.LastLogin,
                IsActive = u.IsActive ?? true,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserDto?> GetByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            return new UserDto
            {
                UserId = user.UserId,
                EmployeeId = user.EmployeeId,
                EmployeeName = $"{user.Employee.FirstName} {user.Employee.LastName}",
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                Username = user.Username,
                Email = user.Email,
                LastLogin = user.LastLogin,
                IsActive = user.IsActive ?? true,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto> AddAsync(CreateUserDto dto)
        {
            if (await _userRepository.UsernameExistsAsync(dto.Username))
                throw new ConflictException("Username already exists.");

            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new ConflictException("Email already exists.");

            if (!await _userRepository.EmployeeExistsAsync(dto.EmployeeId))
                throw new BadRequestException("Employee does not exist.");

            if (!await _userRepository.RoleExistsAsync(dto.RoleId))
                throw new BadRequestException("Role does not exist.");

            if (await _userRepository.EmployeeHasUserAsync(dto.EmployeeId))
                throw new ConflictException("This employee already has a user account.");

            var user = new User
            {
                EmployeeId = dto.EmployeeId,
                RoleId = dto.RoleId,
                Username = dto.Username.Trim(),
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                Email = dto.Email.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var createdUser = await _userRepository.GetByIdAsync(user.UserId);

            return new UserDto
            {
                UserId = createdUser!.UserId,
                EmployeeId = createdUser.EmployeeId,
                EmployeeName = $"{createdUser.Employee.FirstName} {createdUser.Employee.LastName}",
                RoleId = createdUser.RoleId,
                RoleName = createdUser.Role.RoleName,
                Username = createdUser.Username,
                Email = createdUser.Email,
                LastLogin = createdUser.LastLogin,
                IsActive = createdUser.IsActive ?? true,
                CreatedAt = createdUser.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(int userId, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return false;

            if (!string.Equals(user.Username, dto.Username, StringComparison.OrdinalIgnoreCase)
                && await _userRepository.UsernameExistsAsync(dto.Username))
            {
                throw new ConflictException("Username already exists.");
            }

            if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase)
                && await _userRepository.EmailExistsAsync(dto.Email))
            {
                throw new ConflictException("Email already exists.");
            }

            if (!await _userRepository.RoleExistsAsync(dto.RoleId))
                throw new BadRequestException("Role does not exist.");

            user.RoleId = dto.RoleId;
            user.Username = dto.Username.Trim();
            user.Email = dto.Email.Trim();
            user.IsActive = dto.IsActive;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return false;

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }
    }
}