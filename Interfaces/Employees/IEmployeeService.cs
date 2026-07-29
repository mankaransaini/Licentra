using Licentra.API.DTOs.Employees;

namespace Licentra.API.Interfaces.Employees
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();

        Task<EmployeeDto?> GetByIdAsync(int employeeId);

        Task<EmployeeDto> AddAsync(CreateEmployeeDto createEmployeeDto);

        Task<bool> UpdateAsync(int employeeId, UpdateEmployeeDto updateEmployeeDto);

        Task<bool> DeleteAsync(int employeeId);
    }
}