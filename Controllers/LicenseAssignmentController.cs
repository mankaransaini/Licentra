using Licentra.API.Common.Responses;
using Licentra.API.DTOs.LicenseAssignments;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.LicenseAssignments;
using Licentra.API.Interfaces.Users;
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
        private readonly IUserRepository _userRepository;

        public LicenseAssignmentController(
            ILicenseAssignmentService licenseAssignmentService,
            IUserRepository userRepository)
        {
            _licenseAssignmentService = licenseAssignmentService;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet("my-assignments")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LicenseAssignmentDto>>>> GetMyAssignments()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value
                           ?? User.FindFirst("UserId")?.Value;

            int employeeId = 0;
            var empIdClaim = User.FindFirst("employeeId")?.Value ?? User.FindFirst("EmployeeId")?.Value;
            if (int.TryParse(empIdClaim, out int parsedEmpId) && parsedEmpId > 0)
            {
                employeeId = parsedEmpId;
            }
            else if (int.TryParse(userIdClaim, out int uid) && uid > 0)
            {
                var userObj = await _userRepository.GetByIdAsync(uid);
                if (userObj != null)
                {
                    employeeId = userObj.EmployeeId;
                }
            }

            if (employeeId <= 0)
            {
                return Ok(new ApiResponse<IEnumerable<LicenseAssignmentDto>>(
                    true,
                    "No employee profile linked.",
                    new List<LicenseAssignmentDto>()
                ));
            }

            var assignments = await _licenseAssignmentService.GetByEmployeeIdAsync(employeeId);

            return Ok(new ApiResponse<IEnumerable<LicenseAssignmentDto>>(
                true,
                "My assignments retrieved successfully.",
                assignments
            ));
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

        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LicenseAssignmentDto>>>> GetAssignmentsByEmployeeId(int employeeId)
        {
            var assignments = await _licenseAssignmentService.GetByEmployeeIdAsync(employeeId);

            return Ok(new ApiResponse<IEnumerable<LicenseAssignmentDto>>(
                true,
                "Employee assignments retrieved successfully.",
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