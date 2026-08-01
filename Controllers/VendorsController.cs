using Licentra.API.Common.Responses;
using Licentra.API.DTOs.Vendors;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.Vendors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorsController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<VendorDto>>>> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<VendorDto>>(
                true,
                "Vendors retrieved successfully.",
                vendors
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<VendorDto>>> GetVendorById(int id)
        {
            var vendor = await _vendorService.GetByIdAsync(id);

            if (vendor == null)
                throw new NotFoundException("Vendor not found.");

            return Ok(new ApiResponse<VendorDto>(
                true,
                "Vendor retrieved successfully.",
                vendor
            ));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VendorDto>>> CreateVendor(CreateVendorDto dto)
        {
            var vendor = await _vendorService.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetVendorById),
                new { id = vendor.VendorId },
                new ApiResponse<VendorDto>(
                    true,
                    "Vendor created successfully.",
                    vendor
                ));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateVendor(int id, UpdateVendorDto dto)
        {
            var updated = await _vendorService.UpdateAsync(id, dto);

            if (!updated)
                throw new NotFoundException("Vendor not found.");

            return Ok(new ApiResponse<object>(
                true,
                "Vendor updated successfully.",
                null
            ));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteVendor(int id)
        {
            var deleted = await _vendorService.DeleteAsync(id);

            if (!deleted)
                throw new NotFoundException("Vendor not found.");

            return Ok(new ApiResponse<object>(
                true,
                "Vendor deleted successfully.",
                null
            ));
        }
    }
}