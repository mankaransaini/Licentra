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
            var user = await _userRepository.GetByUsernameAsync(dto.Username);

            if (user == null)
                throw new UnauthorizedException("Invalid username or password.");

            if (!(user.IsActive ?? false))
                throw new UnauthorizedException("User account is inactive.");

            bool isValidPassword = _passwordHasher.VerifyPassword(
                dto.Password,
                user.PasswordHash);

            if (!isValidPassword)
                throw new UnauthorizedException("Invalid username or password.");

            user.LastLogin = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            string token = _jwtTokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60),
                Username = user.Username,
                Role = user.Role.RoleName
            };
        }
    }
}