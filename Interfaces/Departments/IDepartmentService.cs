using Licentra.API.DTOs.Departments;

namespace Licentra.API.Interfaces.Departments
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();

        Task<DepartmentDto?> GetByIdAsync(int departmentId);

        Task<DepartmentDto> AddAsync(CreateDepartmentDto createDepartmentDto);

        Task<bool> UpdateAsync(int departmentId, UpdateDepartmentDto updateDepartmentDto);

        Task<bool> DeleteAsync(int departmentId);
    }
}