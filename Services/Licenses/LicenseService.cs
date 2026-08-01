using Licentra.API.DTOs.Licenses;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Interfaces.Licenses;
using Licentra.API.Models;

namespace Licentra.API.Services.Licenses
{
    public class LicenseService : ILicenseService
    {
        private readonly ILicenseRepository _licenseRepository;
        private readonly IAuditLogService _auditLogService;

        public LicenseService(
            ILicenseRepository licenseRepository,
            IAuditLogService auditLogService)
        {
            _licenseRepository = licenseRepository ??
                throw new ArgumentNullException(nameof(licenseRepository));

            _auditLogService = auditLogService ??
                throw new ArgumentNullException(nameof(auditLogService));
        }

        public async Task<IEnumerable<LicenseDto>> GetAllAsync()
        {
            var licenses = await _licenseRepository.GetAllAsync();

            return licenses.Select(l => new LicenseDto
            {
                LicenseId = l.LicenseId,
                SoftwareId = l.SoftwareId,
                SoftwareName = l.Software.SoftwareName,
                LicenseKey = l.LicenseKey,
                LicenseType = l.LicenseType,
                PurchaseDate = l.PurchaseDate,
                ExpiryDate = l.ExpiryDate,
                PurchaseCost = l.PurchaseCost,
                Seats = l.Seats,
                LicenseStatus = l.LicenseStatus,
                Notes = l.Notes,
                CreatedAt = l.CreatedAt
            });
        }

        public async Task<LicenseDto?> GetByIdAsync(int licenseId)
        {
            var license = await _licenseRepository.GetByIdAsync(licenseId);

            if (license == null)
                return null;

            return new LicenseDto
            {
                LicenseId = license.LicenseId,
                SoftwareId = license.SoftwareId,
                SoftwareName = license.Software.SoftwareName,
                LicenseKey = license.LicenseKey,
                LicenseType = license.LicenseType,
                PurchaseDate = license.PurchaseDate,
                ExpiryDate = license.ExpiryDate,
                PurchaseCost = license.PurchaseCost,
                Seats = license.Seats,
                LicenseStatus = license.LicenseStatus,
                Notes = license.Notes,
                CreatedAt = license.CreatedAt
            };
        }

        public async Task<LicenseDto> AddAsync(CreateLicenseDto dto)
        {
            if (!await _licenseRepository.SoftwareExistsAsync(dto.SoftwareId))
                throw new BadRequestException("Software does not exist.");

            if (await _licenseRepository.LicenseKeyExistsAsync(dto.LicenseKey))
                throw new ConflictException("License key already exists.");

            if (dto.ExpiryDate.HasValue &&
                dto.PurchaseDate > dto.ExpiryDate.Value)
                throw new BadRequestException("Purchase date cannot be after expiry date.");

            if (dto.PurchaseCost < 0)
                throw new BadRequestException("Purchase cost cannot be negative.");

            if (dto.Seats <= 0)
                throw new BadRequestException("Seats must be greater than zero.");

            var license = new License
            {
                SoftwareId = dto.SoftwareId,
                LicenseKey = dto.LicenseKey.Trim(),
                LicenseType = dto.LicenseType.Trim(),
                PurchaseDate = dto.PurchaseDate,
                ExpiryDate = dto.ExpiryDate,
                PurchaseCost = dto.PurchaseCost,
                Seats = dto.Seats,
                LicenseStatus = dto.LicenseStatus,
                Notes = dto.Notes?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _licenseRepository.AddAsync(license);
            await _licenseRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "CREATE",
                "License",
                license.LicenseId,
                $"Created license '{license.LicenseKey}'"
            );

            var created = await _licenseRepository.GetByIdAsync(license.LicenseId);

            return new LicenseDto
            {
                LicenseId = created!.LicenseId,
                SoftwareId = created.SoftwareId,
                SoftwareName = created.Software.SoftwareName,
                LicenseKey = created.LicenseKey,
                LicenseType = created.LicenseType,
                PurchaseDate = created.PurchaseDate,
                ExpiryDate = created.ExpiryDate,
                PurchaseCost = created.PurchaseCost,
                Seats = created.Seats,
                LicenseStatus = created.LicenseStatus,
                Notes = created.Notes,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(int licenseId, UpdateLicenseDto dto)
        {
            var license = await _licenseRepository.GetByIdAsync(licenseId);

            if (license == null)
                return false;

            if (!await _licenseRepository.SoftwareExistsAsync(dto.SoftwareId))
                throw new BadRequestException("Software does not exist.");

            bool isDuplicate =
                !string.Equals(license.LicenseKey, dto.LicenseKey, StringComparison.OrdinalIgnoreCase);

            if (isDuplicate &&
                await _licenseRepository.LicenseKeyExistsAsync(dto.LicenseKey))
            {
                throw new ConflictException("License key already exists.");
            }

            if (dto.ExpiryDate.HasValue &&
                dto.PurchaseDate > dto.ExpiryDate.Value)
                throw new BadRequestException("Purchase date cannot be after expiry date.");

            if (dto.PurchaseCost < 0)
                throw new BadRequestException("Purchase cost cannot be negative.");

            if (dto.Seats <= 0)
                throw new BadRequestException("Seats must be greater than zero.");

            license.SoftwareId = dto.SoftwareId;
            license.LicenseKey = dto.LicenseKey.Trim();
            license.LicenseType = dto.LicenseType.Trim();
            license.PurchaseDate = dto.PurchaseDate;
            license.ExpiryDate = dto.ExpiryDate;
            license.PurchaseCost = dto.PurchaseCost;
            license.Seats = dto.Seats;
            license.LicenseStatus = dto.LicenseStatus;
            license.Notes = dto.Notes?.Trim();

            await _licenseRepository.UpdateAsync(license);
            await _licenseRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "UPDATE",
                "License",
                license.LicenseId,
                $"Updated license '{license.LicenseKey}'"
            );

            return true;
        }

        public async Task<bool> DeleteAsync(int licenseId)
        {
            var license = await _licenseRepository.GetByIdAsync(licenseId);

            if (license == null)
                return false;

            string licenseKey = license.LicenseKey;

            await _licenseRepository.DeleteAsync(license);
            await _licenseRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "DELETE",
                "License",
                license.LicenseId,
                $"Deleted license '{licenseKey}'"
            );

            return true;
        }
    }
}