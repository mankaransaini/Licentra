using Licentra.API.Common.Responses;
using Licentra.API.DTOs.Employees;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Licentra.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllAsync();

            return Ok(new ApiResponse<IEnumerable<EmployeeDto>>(
                true,
                "Employees retrieved successfully.",
                employees
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                throw new NotFoundException("Employee not found.");
            }

            return Ok(new ApiResponse<EmployeeDto>(
                true,
                "Employee retrieved successfully.",
                employee
            ));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDto dto)
        {
            try
            {
                var employee = await _employeeService.AddAsync(dto);

                return Ok(new ApiResponse<EmployeeDto>(
                    true,
                    "Employee created successfully.",
                    employee
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    ex.Message,
                    null
                ));
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            var updated = await _employeeService.UpdateAsync(id, dto);

            if (!updated)
            {
                throw new NotFoundException("Employee not found.");
            }

            return Ok(new ApiResponse<object>(
                true,
                "Employee updated successfully.",
                null
            ));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteEmployee(int id)
        {
            var deleted = await _employeeService.DeleteAsync(id);

            if (!deleted)
            {
                throw new NotFoundException("Employee not found.");
            }

            return Ok(new ApiResponse<object>(
                true,
                "Employee deleted successfully.",
                null
            ));
        }
    }
}