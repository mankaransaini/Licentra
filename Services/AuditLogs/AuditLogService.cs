using Licentra.API.DTOs.AuditLogs;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Licentra.API.Services.AuditLogs
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(
            IAuditLogRepository auditLogRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _auditLogRepository = auditLogRepository ??
                throw new ArgumentNullException(nameof(auditLogRepository));

            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
        {
            var logs = await _auditLogRepository.GetAllAsync();

            return logs.Select(log => new AuditLogDto
            {
                AuditLogId = log.AuditLogId,
                UserId = log.UserId,
                Username = log.User.Username,
                Action = log.Action,
                TableName = log.TableName,
                RecordId = log.RecordId,
                Description = log.Description,
                ActionDate = log.ActionDate
            });
        }

        public async Task<AuditLogDto?> GetByIdAsync(int auditLogId)
        {
            var log = await _auditLogRepository.GetByIdAsync(auditLogId);

            if (log == null)
                return null;

            return new AuditLogDto
            {
                AuditLogId = log.AuditLogId,
                UserId = log.UserId,
                Username = log.User.Username,
                Action = log.Action,
                TableName = log.TableName,
                RecordId = log.RecordId,
                Description = log.Description,
                ActionDate = log.ActionDate
            };
        }

        public async Task LogAsync(
    string action,
    string tableName,
    int recordId,
    string description)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (!int.TryParse(userIdClaim, out int userId))
                return;

            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                Description = description,
                ActionDate = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(auditLog);
            await _auditLogRepository.SaveChangesAsync();
        }
    }
}