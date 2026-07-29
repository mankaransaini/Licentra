using Licentra.API.Models;

namespace Licentra.API.Interfaces.Users
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByUsernameAsync(string username);

        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);

        Task<bool> ExistsAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> EmployeeExistsAsync(int employeeId);
        Task<bool> RoleExistsAsync(int roleId);
        Task<bool> EmployeeHasUserAsync(int employeeId);

        Task SaveChangesAsync();
    }
}