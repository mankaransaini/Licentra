using Licentra.API.Data;
using Licentra.API.Interfaces.Employees;
using Licentra.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Repositories.Employees
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly LicentraDbContext _context;

        public EmployeeRepository(LicentraDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int employeeId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentId == departmentId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Employees
                .AnyAsync(e => e.Email == email);
        }

        public async Task<bool> EmployeeCodeExistsAsync(string employeeCode)
        {
            return await _context.Employees
                .AnyAsync(e => e.EmployeeCode == employeeCode);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}