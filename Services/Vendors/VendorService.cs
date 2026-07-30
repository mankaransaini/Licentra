using Licentra.API.DTOs.Vendors;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Interfaces.Vendors;
using Licentra.API.Models;

namespace Licentra.API.Services.Vendors
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IAuditLogService _auditLogService;

        public VendorService(
            IVendorRepository vendorRepository,
            IAuditLogService auditLogService)
        {
            _vendorRepository = vendorRepository ??
                throw new ArgumentNullException(nameof(vendorRepository));

            _auditLogService = auditLogService ??
                throw new ArgumentNullException(nameof(auditLogService));
        }

        public async Task<IEnumerable<VendorDto>> GetAllAsync()
        {
            var vendors = await _vendorRepository.GetAllAsync();

            return vendors.Select(v => new VendorDto
            {
                VendorId = v.VendorId,
                VendorName = v.VendorName,
                ContactPerson = v.ContactPerson,
                Email = v.Email,
                Phone = v.Phone,
                Website = v.Website,
                Address = v.Address,
                IsActive = v.IsActive ?? true,
                CreatedAt = v.CreatedAt
            });
        }

        public async Task<VendorDto?> GetByIdAsync(int vendorId)
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);

            if (vendor == null)
                return null;

            return new VendorDto
            {
                VendorId = vendor.VendorId,
                VendorName = vendor.VendorName,
                ContactPerson = vendor.ContactPerson,
                Email = vendor.Email,
                Phone = vendor.Phone,
                Website = vendor.Website,
                Address = vendor.Address,
                IsActive = vendor.IsActive ?? true,
                CreatedAt = vendor.CreatedAt
            };
        }

        public async Task<VendorDto> AddAsync(CreateVendorDto dto)
        {
            if (await _vendorRepository.VendorNameExistsAsync(dto.VendorName))
                throw new ConflictException("Vendor name already exists.");

            var vendor = new Vendor
            {
                VendorName = dto.VendorName.Trim(),
                ContactPerson = dto.ContactPerson?.Trim(),
                Email = dto.Email?.Trim(),
                Phone = dto.Phone?.Trim(),
                Website = dto.Website?.Trim(),
                Address = dto.Address?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _vendorRepository.AddAsync(vendor);
            await _vendorRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "CREATE",
                "Vendor",
                vendor.VendorId,
                $"Created vendor '{vendor.VendorName}'"
            );

            return new VendorDto
            {
                VendorId = vendor.VendorId,
                VendorName = vendor.VendorName,
                ContactPerson = vendor.ContactPerson,
                Email = vendor.Email,
                Phone = vendor.Phone,
                Website = vendor.Website,
                Address = vendor.Address,
                IsActive = vendor.IsActive ?? true,
                CreatedAt = vendor.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(int vendorId, UpdateVendorDto dto)
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);

            if (vendor == null)
                return false;

            if (!string.Equals(vendor.VendorName, dto.VendorName, StringComparison.OrdinalIgnoreCase)
                && await _vendorRepository.VendorNameExistsAsync(dto.VendorName))
            {
                throw new ConflictException("Vendor name already exists.");
            }

            vendor.VendorName = dto.VendorName.Trim();
            vendor.ContactPerson = dto.ContactPerson?.Trim();
            vendor.Email = dto.Email?.Trim();
            vendor.Phone = dto.Phone?.Trim();
            vendor.Website = dto.Website?.Trim();
            vendor.Address = dto.Address?.Trim();
            vendor.IsActive = dto.IsActive;

            await _vendorRepository.UpdateAsync(vendor);
            await _vendorRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "UPDATE",
                "Vendor",
                vendor.VendorId,
                $"Updated vendor '{vendor.VendorName}'"
            );

            return true;
        }

        public async Task<bool> DeleteAsync(int vendorId)
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);

            if (vendor == null)
                return false;

            string vendorName = vendor.VendorName;

            await _vendorRepository.DeleteAsync(vendor);
            await _vendorRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "DELETE",
                "Vendor",
                vendor.VendorId,
                $"Deleted vendor '{vendorName}'"
            );

            return true;
        }
    }
}