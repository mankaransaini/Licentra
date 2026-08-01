using Licentra.API.DTOs.Auth;

namespace Licentra.API.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}