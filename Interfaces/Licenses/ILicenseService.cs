using Licentra.API.DTOs.Licenses;

namespace Licentra.API.Interfaces.Licenses
{
    public interface ILicenseService
    {
        Task<IEnumerable<LicenseDto>> GetAllAsync();

        Task<LicenseDto?> GetByIdAsync(int licenseId);

        Task<LicenseDto> AddAsync(CreateLicenseDto dto);

        Task<bool> UpdateAsync(int licenseId, UpdateLicenseDto dto);

        Task<bool> DeleteAsync(int licenseId);
    }
}