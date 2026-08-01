using Licentra.API.Models;

namespace Licentra.API.Interfaces.LicenseAssignments
{
    public interface ILicenseAssignmentRepository
    {
        Task<IEnumerable<LicenseAssignment>> GetAllAsync();

        Task<LicenseAssignment?> GetByIdAsync(int assignmentId);

        Task AddAsync(LicenseAssignment assignment);

        Task UpdateAsync(LicenseAssignment assignment);

        Task DeleteAsync(LicenseAssignment assignment);

        Task<bool> ExistsAsync(int assignmentId);

        Task<bool> LicenseExistsAsync(int licenseId);

        Task<bool> EmployeeExistsAsync(int employeeId);

        Task<bool> UserExistsAsync(int userId);

        Task<int> GetFirstValidUserIdAsync();

        Task<bool> ActiveAssignmentExistsAsync(int licenseId, int employeeId);

        Task SaveChangesAsync();
    }
}