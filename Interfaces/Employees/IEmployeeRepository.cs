using Licentra.API.Models;

namespace Licentra.API.Interfaces.Employees
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();

        Task<Employee?> GetByIdAsync(int employeeId);

        Task<Employee?> GetByEmailAsync(string email);

        Task AddAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task DeleteAsync(Employee employee);

        Task<bool> ExistsAsync(int employeeId);

        Task<bool> DepartmentExistsAsync(int departmentId);

        Task<bool> EmailExistsAsync(string email);

        Task SaveChangesAsync();
        Task<bool> EmployeeCodeExistsAsync(string employeeCode);

    }
}