using Licentra.API.Models;

namespace Licentra.API.Interfaces.Software
{
    public interface ISoftwareRepository
    {
        Task<IEnumerable<Models.Software>> GetAllAsync();

        Task<Models.Software?> GetByIdAsync(int softwareId);

        Task AddAsync(Models.Software software);

        Task UpdateAsync(Models.Software software);

        Task DeleteAsync(Models.Software software);

        Task<bool> ExistsAsync(int softwareId);

        Task<bool> VendorExistsAsync(int vendorId);

        Task<bool> SoftwareExistsAsync(string softwareName, string version);

        Task SaveChangesAsync();
    }
}