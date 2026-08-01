using Licentra.API.DTOs.AuditLogs;

namespace Licentra.API.Interfaces.AuditLogs
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogDto>> GetAllAsync();

        Task<AuditLogDto?> GetByIdAsync(int auditLogId);

        Task LogAsync(
            string action,
            string tableName,
            int recordId,
            string description);
    }
}