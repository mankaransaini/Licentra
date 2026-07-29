using Licentra.API.Models;

namespace Licentra.API.Interfaces.Licenses
{
    public interface ILicenseRepository
    {
        Task<IEnumerable<License>> GetAllAsync();

        Task<License?> GetByIdAsync(int licenseId);

        Task AddAsync(License license);

        Task UpdateAsync(License license);

        Task DeleteAsync(License license);

        Task<bool> ExistsAsync(int licenseId);

        Task<bool> SoftwareExistsAsync(int softwareId);

        Task<bool> LicenseKeyExistsAsync(string licenseKey);

        Task SaveChangesAsync();
    }
}