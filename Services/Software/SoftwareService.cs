using Licentra.API.DTOs.Software;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Interfaces.Software;

namespace Licentra.API.Services.Software
{
    public class SoftwareService : ISoftwareService
    {
        private readonly ISoftwareRepository _softwareRepository;
        private readonly IAuditLogService _auditLogService;

        public SoftwareService(
            ISoftwareRepository softwareRepository,
            IAuditLogService auditLogService)
        {
            _softwareRepository = softwareRepository ??
                throw new ArgumentNullException(nameof(softwareRepository));

            _auditLogService = auditLogService ??
                throw new ArgumentNullException(nameof(auditLogService));
        }

        public async Task<IEnumerable<SoftwareDto>> GetAllAsync()
        {
            var softwares = await _softwareRepository.GetAllAsync();

            return softwares.Select(s => new SoftwareDto
            {
                SoftwareId = s.SoftwareId,
                VendorId = s.VendorId,
                VendorName = s.Vendor.VendorName,
                SoftwareName = s.SoftwareName,
                Version = s.Version,
                Category = s.Category,
                Description = s.Description,
                IsSubscription = s.IsSubscription ?? false,
                IsActive = s.IsActive ?? true,
                CreatedAt = s.CreatedAt
            });
        }

        public async Task<SoftwareDto?> GetByIdAsync(int softwareId)
        {
            var software = await _softwareRepository.GetByIdAsync(softwareId);

            if (software == null)
                return null;

            return new SoftwareDto
            {
                SoftwareId = software.SoftwareId,
                VendorId = software.VendorId,
                VendorName = software.Vendor.VendorName,
                SoftwareName = software.SoftwareName,
                Version = software.Version,
                Category = software.Category,
                Description = software.Description,
                IsSubscription = software.IsSubscription ?? false,
                IsActive = software.IsActive ?? true,
                CreatedAt = software.CreatedAt
            };
        }

        public async Task<SoftwareDto> AddAsync(CreateSoftwareDto dto)
        {
            if (!await _softwareRepository.VendorExistsAsync(dto.VendorId))
                throw new BadRequestException("Vendor does not exist.");

            if (await _softwareRepository.SoftwareExistsAsync(dto.SoftwareName, dto.Version))
                throw new ConflictException("Software with this version already exists.");

            var software = new Models.Software
            {
                VendorId = dto.VendorId,
                SoftwareName = dto.SoftwareName.Trim(),
                Version = dto.Version.Trim(),
                Category = dto.Category?.Trim(),
                Description = dto.Description?.Trim(),
                IsSubscription = dto.IsSubscription,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _softwareRepository.AddAsync(software);
            await _softwareRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "CREATE",
                "Software",
                software.SoftwareId,
                $"Created software '{software.SoftwareName} {software.Version}'"
            );

            var createdSoftware = await _softwareRepository.GetByIdAsync(software.SoftwareId);

            return new SoftwareDto
            {
                SoftwareId = createdSoftware!.SoftwareId,
                VendorId = createdSoftware.VendorId,
                VendorName = createdSoftware.Vendor.VendorName,
                SoftwareName = createdSoftware.SoftwareName,
                Version = createdSoftware.Version,
                Category = createdSoftware.Category,
                Description = createdSoftware.Description,
                IsSubscription = createdSoftware.IsSubscription ?? false,
                IsActive = createdSoftware.IsActive ?? true,
                CreatedAt = createdSoftware.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(int softwareId, UpdateSoftwareDto dto)
        {
            var software = await _softwareRepository.GetByIdAsync(softwareId);

            if (software == null)
                return false;

            if (!await _softwareRepository.VendorExistsAsync(dto.VendorId))
                throw new BadRequestException("Vendor does not exist.");

            bool isDuplicate =
                !string.Equals(software.SoftwareName, dto.SoftwareName, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(software.Version, dto.Version, StringComparison.OrdinalIgnoreCase);

            if (isDuplicate &&
                await _softwareRepository.SoftwareExistsAsync(dto.SoftwareName, dto.Version))
            {
                throw new ConflictException("Software with this version already exists.");
            }

            software.VendorId = dto.VendorId;
            software.SoftwareName = dto.SoftwareName.Trim();
            software.Version = dto.Version.Trim();
            software.Category = dto.Category?.Trim();
            software.Description = dto.Description?.Trim();
            software.IsSubscription = dto.IsSubscription;
            software.IsActive = dto.IsActive;

            await _softwareRepository.UpdateAsync(software);
            await _softwareRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "UPDATE",
                "Software",
                software.SoftwareId,
                $"Updated software '{software.SoftwareName} {software.Version}'"
            );

            return true;
        }

        public async Task<bool> DeleteAsync(int softwareId)
        {
            var software = await _softwareRepository.GetByIdAsync(softwareId);

            if (software == null)
                return false;

            string softwareName = $"{software.SoftwareName} {software.Version}";

            await _softwareRepository.DeleteAsync(software);
            await _softwareRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "DELETE",
                "Software",
                software.SoftwareId,
                $"Deleted software '{softwareName}'"
            );

            return true;
        }
    }
}