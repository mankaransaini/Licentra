using Licentra.API.Models;

namespace Licentra.API.Interfaces.Vendors
{
    public interface IVendorRepository
    {
        Task<IEnumerable<Vendor>> GetAllAsync();
        Task<Vendor?> GetByIdAsync(int vendorId);
        Task<Vendor?> GetByNameAsync(string vendorName);

        Task AddAsync(Vendor vendor);
        Task UpdateAsync(Vendor vendor);
        Task DeleteAsync(Vendor vendor);

        Task<bool> ExistsAsync(int vendorId);
        Task<bool> VendorNameExistsAsync(string vendorName);

        Task SaveChangesAsync();
    }
}