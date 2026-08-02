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
                Username = log.User?.Username ?? "Admin",
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
                Username = log.User?.Username ?? "Admin",
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
            try
            {
                var userClaims = _httpContextAccessor.HttpContext?.User;
                string? idStr = userClaims?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? userClaims?.FindFirst("sub")?.Value
                             ?? userClaims?.FindFirst("UserId")?.Value;

                int userId = 0;
                if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out int parsedId) && parsedId > 0)
                {
                    if (await _auditLogRepository.UserExistsAsync(parsedId))
                    {
                        userId = parsedId;
                    }
                }

                if (userId <= 0)
                {
                    if (await _auditLogRepository.UserExistsAsync(1))
                    {
                        userId = 1;
                    }
                    else
                    {
                        var firstUserId = await _auditLogRepository.GetFirstUserIdAsync();
                        if (firstUserId.HasValue)
                        {
                            userId = firstUserId.Value;
                        }
                    }
                }

                if (userId <= 0)
                {
                    return;
                }

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
                Console.WriteLine($"[AUDIT LOG SUCCESS] Logged {action} on {tableName} #{recordId} for User #{userId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AUDIT LOG ERROR] Failed to record audit log: {ex}");
            }
        }
    }
}