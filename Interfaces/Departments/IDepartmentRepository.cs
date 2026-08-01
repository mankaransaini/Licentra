using Licentra.API.Models;

namespace Licentra.API.Interfaces.Departments
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();

        Task<Department?> GetByIdAsync(int departmentId);

        Task<Department?> GetByNameAsync(string departmentName);

        Task AddAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(Department department);

        Task<bool> ExistsAsync(int departmentId);

        Task<bool> SaveChangesAsync();
    }
}