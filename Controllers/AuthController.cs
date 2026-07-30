using Licentra.API.Common.Responses;
using Licentra.API.DTOs.Auth;
using Licentra.API.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(LoginRequestDto dto)
        {
            var response = await _authService.LoginAsync(dto);

            return Ok(new ApiResponse<LoginResponseDto>(
                true,
                "Login successful.",
                response
            ));
        }
    }
}