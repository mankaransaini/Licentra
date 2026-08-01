using Licentra.API.Models;
namespace Licentra.API.Interfaces.Security
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}