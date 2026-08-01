using Licentra.API.Common.Responses;
using Licentra.API.DTOs.LicenseAssignments;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.LicenseAssignments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseAssignmentController : ControllerBase
    {
        private readonly ILicenseAssignmentService _licenseAssignmentService;

        public LicenseAssignmentController(ILicenseAssignmentService licenseAssignmentService)
        {
            _licenseAssignmentService = licenseAssignmentService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<LicenseAssignmentDto>>>> GetAllAssignments()
        {
            var assignments = await _licenseAssignmentService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<LicenseAssignmentDto>>(
                true,
                "License assignments retrieved successfully.",
                assignments
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LicenseAssignmentDto>>> GetAssignmentById(int id)
        {
            var assignment = await _licenseAssignmentService.GetByIdAsync(id);

            if (assignment == null)
                throw new NotFoundException("License assignment not found.");

            return Ok(new ApiResponse<LicenseAssignmentDto>(
                true,
                "License assignment retrieved successfully.",
                assignment
            ));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<LicenseAssignmentDto>>> CreateAssignment(CreateLicenseAssignmentDto dto)
        {
            if (dto.AssignedByUserId <= 0)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int uid) && uid > 0)
                {
                    dto.AssignedByUserId = uid;
                }
            }

            var assignment = await _licenseAssignmentService.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetAssignmentById),
                new { id = assignment.AssignmentId },
                new ApiResponse<LicenseAssignmentDto>(
                    true,
                    "License assignment created successfully.",
                    assignment
                ));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateAssignment(int id, UpdateLicenseAssignmentDto dto)
        {
            var updated = await _licenseAssignmentService.UpdateAsync(id, dto);

            if (!updated)
                throw new NotFoundException("License assignment not found.");

            return Ok(new ApiResponse<object>(
                true,
                "License assignment updated successfully.",
                null
            ));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAssignment(int id)
        {
            var deleted = await _licenseAssignmentService.DeleteAsync(id);

            if (!deleted)
                throw new NotFoundException("License assignment not found.");

            return Ok(new ApiResponse<object>(
                true,
                "License assignment deleted successfully.",
                null
            ));
        }
    }
}