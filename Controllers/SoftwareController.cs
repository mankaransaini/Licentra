using Licentra.API.Common.Responses;
using Licentra.API.DTOs.Software;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.Software;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SoftwareController : ControllerBase
    {
        private readonly ISoftwareService _softwareService;

        public SoftwareController(ISoftwareService softwareService)
        {
            _softwareService = softwareService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SoftwareDto>>>> GetAllSoftware()
        {
            var software = await _softwareService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<SoftwareDto>>(
                true,
                "Software retrieved successfully.",
                software
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SoftwareDto>>> GetSoftwareById(int id)
        {
            var software = await _softwareService.GetByIdAsync(id);

            if (software == null)
                throw new NotFoundException("Software not found.");

            return Ok(new ApiResponse<SoftwareDto>(
                true,
                "Software retrieved successfully.",
                software
            ));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SoftwareDto>>> CreateSoftware(CreateSoftwareDto dto)
        {
            var software = await _softwareService.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetSoftwareById),
                new { id = software.SoftwareId },
                new ApiResponse<SoftwareDto>(
                    true,
                    "Software created successfully.",
                    software
                ));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateSoftware(int id, UpdateSoftwareDto dto)
        {
            var updated = await _softwareService.UpdateAsync(id, dto);

            if (!updated)
                throw new NotFoundException("Software not found.");

            return Ok(new ApiResponse<object>(
                true,
                "Software updated successfully.",
                null
            ));
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSoftware(int id)
        {
            var deleted = await _softwareService.DeleteAsync(id);

            if (!deleted)
                throw new NotFoundException("Software not found.");

            return Ok(new ApiResponse<object>(
                true,
                "Software deleted successfully.",
                null
            ));
        }
    }
}