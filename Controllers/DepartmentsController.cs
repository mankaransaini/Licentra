using Licentra.API.DTOs.Departments;
using Licentra.API.Interfaces.Departments;
using Microsoft.AspNetCore.Mvc;
using Licentra.API.Common.Responses;
using Microsoft.AspNetCore.Authorization;
namespace Licentra.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
        {
            var departments = await _departmentService.GetAllAsync();
            return Ok(
    new ApiResponse<IEnumerable<DepartmentDto>>(
        true,
        "Departments retrieved successfully.",
        departments));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(
    new ApiResponse<DepartmentDto>(
        true,
        "Department retrieved successfully.",
        department));
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> Create(CreateDepartmentDto createDepartmentDto)
        {
            var department = await _departmentService.AddAsync(createDepartmentDto);

            return CreatedAtAction(
    nameof(GetById),
    new { departmentId = department.DepartmentId },
    new ApiResponse<DepartmentDto>(
        true,
        "Department created successfully.",
        department));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateDepartmentDto updateDepartmentDto)
        {
            var updated = await _departmentService.UpdateAsync(id, updateDepartmentDto);

            if (!updated)
            {
                return NotFound();
            }

            return Ok(
    new ApiResponse<object>(
        true,
        "Department updated successfully.",
        null));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _departmentService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok(
    new ApiResponse<object>(
        true,
        "Department deleted successfully.",
        null));
        }
    }
}