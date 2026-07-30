using Licentra.API.Common.Responses;
using Licentra.API.DTOs.AuditLogs;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogDto>>>> GetAllAuditLogs()
        {
            var logs = await _auditLogService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<AuditLogDto>>(
                true,
                "Audit logs retrieved successfully.",
                logs
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AuditLogDto>>> GetAuditLogById(int id)
        {
            var log = await _auditLogService.GetByIdAsync(id);

            if (log == null)
                throw new NotFoundException("Audit log not found.");

            return Ok(new ApiResponse<AuditLogDto>(
                true,
                "Audit log retrieved successfully.",
                log
            ));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AuditLogDto>>> CreateAuditLog(CreateAuditLogDto dto)
        {
            var log = await _auditLogService.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetAuditLogById),
                new { id = log.AuditLogId },
                new ApiResponse<AuditLogDto>(
                    true,
                    "Audit log created successfully.",
                    log
                ));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateAuditLog(int id, UpdateAuditLogDto dto)
        {
            var updated = await _auditLogService.UpdateAsync(id, dto);

            if (!updated)
                throw new NotFoundException("Audit log not found.");

            return Ok(new ApiResponse<object>(
                true,
                "Audit log updated successfully.",
                null
            ));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAuditLog(int id)
        {
            var deleted = await _auditLogService.DeleteAsync(id);

            if (!deleted)
                throw new NotFoundException("Audit log not found.");

            return Ok(new ApiResponse<object>(
                true,
                "Audit log deleted successfully.",
                null
            ));
        }
    }
}