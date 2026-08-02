using Licentra.API.DTOs.Auth;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.Auth;
using Licentra.API.Interfaces.Security;
using Licentra.API.Interfaces.Users;

namespace Licentra.API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            Console.WriteLine($"[AUTH DEBUG] Login attempt for username: '{dto.Username}'");
            var user = await _userRepository.GetByUsernameAsync(dto.Username);

            if (user == null)
            {
                Console.WriteLine($"[AUTH DEBUG] User '{dto.Username}' not found in database.");
                throw new UnauthorizedException($"User '{dto.Username}' not found.");
            }

            Console.WriteLine($"[AUTH DEBUG] User found: '{user.Username}', IsActive={user.IsActive}, StoredHash='{user.PasswordHash}'");

            if (!(user.IsActive ?? false))
            {
                Console.WriteLine($"[AUTH DEBUG] Account '{user.Username}' is inactive.");
                throw new UnauthorizedException("User account is inactive.");
            }

            bool isValidPassword = _passwordHasher.VerifyPassword(
                dto.Password,
                user.PasswordHash);

            if (!isValidPassword)
            {
                Console.WriteLine($"[AUTH DEBUG] Password check failed for user '{user.Username}'. DB StoredHash is '{user.PasswordHash}'.");
                throw new UnauthorizedException("Invalid username or password.");
            }

            user.LastLogin = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            string token = _jwtTokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60),
                Username = user.Username,
                Role = user.Role.RoleName,
                EmployeeId = user.EmployeeId,
                Email = user.Email
            };
        }
    }
}