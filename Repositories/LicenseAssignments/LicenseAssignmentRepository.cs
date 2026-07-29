using Licentra.API.Data;
using Licentra.API.Interfaces.LicenseAssignments;
using Licentra.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Repositories.LicenseAssignments
{
    public class LicenseAssignmentRepository : ILicenseAssignmentRepository
    {
        private readonly LicentraDbContext _context;

        public LicenseAssignmentRepository(LicentraDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LicenseAssignment>> GetAllAsync()
        {
            return await _context.LicenseAssignments
                .Include(la => la.Employee)
                .Include(la => la.License)
                .Include(la => la.AssignedByUser)
                .OrderByDescending(la => la.AssignedDate)
                .ToListAsync();
        }

        public async Task<LicenseAssignment?> GetByIdAsync(int assignmentId)
        {
            return await _context.LicenseAssignments
                .Include(la => la.Employee)
                .Include(la => la.License)
                .Include(la => la.AssignedByUser)
                .FirstOrDefaultAsync(la => la.AssignmentId == assignmentId);
        }

        public async Task AddAsync(LicenseAssignment assignment)
        {
            await _context.LicenseAssignments.AddAsync(assignment);
        }

        public Task UpdateAsync(LicenseAssignment assignment)
        {
            _context.LicenseAssignments.Update(assignment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(LicenseAssignment assignment)
        {
            _context.LicenseAssignments.Remove(assignment);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int assignmentId)
        {
            return await _context.LicenseAssignments
                .AnyAsync(la => la.AssignmentId == assignmentId);
        }

        public async Task<bool> LicenseExistsAsync(int licenseId)
        {
            return await _context.Licenses
                .AnyAsync(l => l.LicenseId == licenseId);
        }

        public async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees
                .AnyAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == userId);
        }

        public async Task<bool> ActiveAssignmentExistsAsync(int licenseId, int employeeId)
        {
            return await _context.LicenseAssignments.AnyAsync(la =>
                la.LicenseId == licenseId &&
                la.EmployeeId == employeeId &&
                la.ReturnedDate == null);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}