using Licentra.API.Common.Responses;
using Licentra.API.DTOs.AuditLogs;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Authorize(Roles = "Administrator")] //exception fileeeee 
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

        
    }
}