using Licentra.API.Data;
using Licentra.API.Interfaces.Users;
using Licentra.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly LicentraDbContext _context;

        public UserRepository(LicentraDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Employee)
                .Include(u => u.Role)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Employee)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Employee)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int userId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == userId);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<bool> RoleExistsAsync(int roleId)
        {
            return await _context.Roles
                .AnyAsync(r => r.RoleId == roleId);
        }

        public async Task<bool> EmployeeHasUserAsync(int employeeId)
        {
            return await _context.Users
                .AnyAsync(u => u.EmployeeId == employeeId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}