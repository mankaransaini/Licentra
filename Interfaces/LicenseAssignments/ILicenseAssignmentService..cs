using Licentra.API.DTOs.LicenseAssignments;

namespace Licentra.API.Interfaces.LicenseAssignments
{
    public interface ILicenseAssignmentService
    {
        Task<IEnumerable<LicenseAssignmentDto>> GetAllAsync();

        Task<IEnumerable<LicenseAssignmentDto>> GetByEmployeeIdAsync(int employeeId);

        Task<LicenseAssignmentDto?> GetByIdAsync(int assignmentId);

        Task<LicenseAssignmentDto> AddAsync(CreateLicenseAssignmentDto dto);

        Task<bool> UpdateAsync(int assignmentId, UpdateLicenseAssignmentDto dto);

        Task<bool> DeleteAsync(int assignmentId);
    }
}