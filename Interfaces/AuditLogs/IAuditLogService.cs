using Licentra.API.DTOs.AuditLogs;

namespace Licentra.API.Interfaces.AuditLogs
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogDto>> GetAllAsync();

        Task<AuditLogDto?> GetByIdAsync(int auditLogId);

        Task<AuditLogDto> AddAsync(CreateAuditLogDto dto);

        Task<bool> UpdateAsync(int auditLogId, UpdateAuditLogDto dto);

        Task<bool> DeleteAsync(int auditLogId);
    }
}