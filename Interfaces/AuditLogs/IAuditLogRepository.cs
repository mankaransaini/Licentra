using Licentra.API.Models;

namespace Licentra.API.Interfaces.AuditLogs
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<AuditLog>> GetAllAsync();

        Task<AuditLog?> GetByIdAsync(int auditLogId);

        Task AddAsync(AuditLog auditLog);

        Task UpdateAsync(AuditLog auditLog);

        Task DeleteAsync(AuditLog auditLog);

        Task<bool> ExistsAsync(int auditLogId);

        Task<bool> UserExistsAsync(int userId);

        Task SaveChangesAsync();
    }
}