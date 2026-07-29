using Licentra.API.Data;
using Licentra.API.Interfaces.Departments;
using Licentra.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Repositories.Departments
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly LicentraDbContext _context;

        public DepartmentRepository(LicentraDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int departmentId)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);
        }

        public async Task<Department?> GetByNameAsync(string departmentName)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentName == departmentName);
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        }

        public Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Department department)
        {
            _context.Departments.Remove(department);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int departmentId)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentId == departmentId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}