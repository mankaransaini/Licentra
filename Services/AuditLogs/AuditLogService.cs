using Licentra.API.DTOs.AuditLogs;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Models;

namespace Licentra.API.Services.AuditLogs
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository ??
                throw new ArgumentNullException(nameof(auditLogRepository));
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

        public async Task<AuditLogDto> AddAsync(CreateAuditLogDto dto)
        {
            if (!await _auditLogRepository.UserExistsAsync(dto.UserId))
                throw new BadRequestException("User does not exist.");

            var auditLog = new AuditLog
            {
                UserId = dto.UserId,
                Action = dto.Action.Trim(),
                TableName = dto.TableName.Trim(),
                RecordId = dto.RecordId,
                Description = dto.Description?.Trim(),
                ActionDate = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(auditLog);
            await _auditLogRepository.SaveChangesAsync();

            var created = await _auditLogRepository.GetByIdAsync(auditLog.AuditLogId);

            return new AuditLogDto
            {
                AuditLogId = created!.AuditLogId,
                UserId = created.UserId,
                Username = created.User.Username,
                Action = created.Action,
                TableName = created.TableName,
                RecordId = created.RecordId,
                Description = created.Description,
                ActionDate = created.ActionDate
            };
        }

        public async Task<bool> UpdateAsync(int auditLogId, UpdateAuditLogDto dto)
        {
            var auditLog = await _auditLogRepository.GetByIdAsync(auditLogId);

            if (auditLog == null)
                return false;

            if (!await _auditLogRepository.UserExistsAsync(dto.UserId))
                throw new BadRequestException("User does not exist.");

            auditLog.UserId = dto.UserId;
            auditLog.Action = dto.Action.Trim();
            auditLog.TableName = dto.TableName.Trim();
            auditLog.RecordId = dto.RecordId;
            auditLog.Description = dto.Description?.Trim();

            await _auditLogRepository.UpdateAsync(auditLog);
            await _auditLogRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int auditLogId)
        {
            var auditLog = await _auditLogRepository.GetByIdAsync(auditLogId);

            if (auditLog == null)
                return false;

            await _auditLogRepository.DeleteAsync(auditLog);
            await _auditLogRepository.SaveChangesAsync();

            return true;
        }
    }
}