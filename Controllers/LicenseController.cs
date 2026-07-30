using Licentra.API.Common.Responses;
using Licentra.API.DTOs.Licenses;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.Licenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<LicenseDto>>>> GetAllLicenses()
        {
            var licenses = await _licenseService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<LicenseDto>>(
                true,
                "Licenses retrieved successfully.",
                licenses
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LicenseDto>>> GetLicenseById(int id)
        {
            var license = await _licenseService.GetByIdAsync(id);

            if (license == null)
                throw new NotFoundException("License not found.");

            return Ok(new ApiResponse<LicenseDto>(
                true,
                "License retrieved successfully.",
                license
            ));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<LicenseDto>>> CreateLicense(CreateLicenseDto dto)
        {
            var license = await _licenseService.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetLicenseById),
                new { id = license.LicenseId },
                new ApiResponse<LicenseDto>(
                    true,
                    "License created successfully.",
                    license
                ));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateLicense(int id, UpdateLicenseDto dto)
        {
            var updated = await _licenseService.UpdateAsync(id, dto);

            if (!updated)
                throw new NotFoundException("License not found.");

            return Ok(new ApiResponse<object>(
                true,
                "License updated successfully.",
                null
            ));
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteLicense(int id)
        {
            var deleted = await _licenseService.DeleteAsync(id);

            if (!deleted)
                throw new NotFoundException("License not found.");

            return Ok(new ApiResponse<object>(
                true,
                "License deleted successfully.",
                null
            ));
        }
    }
}