using Licentra.API.DTOs.Vendors;

namespace Licentra.API.Interfaces.Vendors
{
    public interface IVendorService
    {
        Task<IEnumerable<VendorDto>> GetAllAsync();

        Task<VendorDto?> GetByIdAsync(int vendorId);

        Task<VendorDto> AddAsync(CreateVendorDto dto);

        Task<bool> UpdateAsync(int vendorId, UpdateVendorDto dto);

        Task<bool> DeleteAsync(int vendorId);
    }
}