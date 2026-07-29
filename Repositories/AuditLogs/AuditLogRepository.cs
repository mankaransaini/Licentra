using Licentra.API.Data;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Repositories.AuditLogs
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly LicentraDbContext _context;

        public AuditLogRepository(LicentraDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.ActionDate)
                .ToListAsync();
        }

        public async Task<AuditLog?> GetByIdAsync(int auditLogId)
        {
            return await _context.AuditLogs
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AuditLogId == auditLogId);
        }

        public async Task AddAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
        }

        public Task UpdateAsync(AuditLog auditLog)
        {
            _context.AuditLogs.Update(auditLog);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(AuditLog auditLog)
        {
            _context.AuditLogs.Remove(auditLog);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int auditLogId)
        {
            return await _context.AuditLogs
                .AnyAsync(a => a.AuditLogId == auditLogId);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}