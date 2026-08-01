using Licentra.API.DTOs.LicenseAssignments;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Interfaces.LicenseAssignments;
using Licentra.API.Models;

namespace Licentra.API.Services.LicenseAssignments
{
    public class LicenseAssignmentService : ILicenseAssignmentService
    {
        private readonly ILicenseAssignmentRepository _licenseAssignmentRepository;
        private readonly IAuditLogService _auditLogService;

        public LicenseAssignmentService(
            ILicenseAssignmentRepository licenseAssignmentRepository,
            IAuditLogService auditLogService)
        {
            _licenseAssignmentRepository = licenseAssignmentRepository ??
                throw new ArgumentNullException(nameof(licenseAssignmentRepository));

            _auditLogService = auditLogService ??
                throw new ArgumentNullException(nameof(auditLogService));
        }

        public async Task<IEnumerable<LicenseAssignmentDto>> GetAllAsync()
        {
            var assignments = await _licenseAssignmentRepository.GetAllAsync();

            return assignments.Select(a => new LicenseAssignmentDto
            {
                AssignmentId = a.AssignmentId,
                LicenseId = a.LicenseId,
                LicenseKey = a.License.LicenseKey,
                EmployeeId = a.EmployeeId,
                EmployeeName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                AssignedDate = a.AssignedDate,
                ReturnedDate = a.ReturnedDate,
                AssignmentStatus = a.AssignmentStatus,
                AssignedByUserId = a.AssignedByUserId,
                AssignedByUsername = a.AssignedByUser.Username,
                Remarks = a.Remarks
            });
        }

        public async Task<LicenseAssignmentDto?> GetByIdAsync(int assignmentId)
        {
            var assignment = await _licenseAssignmentRepository.GetByIdAsync(assignmentId);

            if (assignment == null)
                return null;

            return new LicenseAssignmentDto
            {
                AssignmentId = assignment.AssignmentId,
                LicenseId = assignment.LicenseId,
                LicenseKey = assignment.License.LicenseKey,
                EmployeeId = assignment.EmployeeId,
                EmployeeName = $"{assignment.Employee.FirstName} {assignment.Employee.LastName}",
                AssignedDate = assignment.AssignedDate,
                ReturnedDate = assignment.ReturnedDate,
                AssignmentStatus = assignment.AssignmentStatus,
                AssignedByUserId = assignment.AssignedByUserId,
                AssignedByUsername = assignment.AssignedByUser.Username,
                Remarks = assignment.Remarks
            };
        }

        public async Task<LicenseAssignmentDto> AddAsync(CreateLicenseAssignmentDto dto)
        {
            if (!await _licenseAssignmentRepository.LicenseExistsAsync(dto.LicenseId))
                throw new BadRequestException("License does not exist.");

            if (!await _licenseAssignmentRepository.EmployeeExistsAsync(dto.EmployeeId))
                throw new BadRequestException("Employee does not exist.");

            if (!await _licenseAssignmentRepository.UserExistsAsync(dto.AssignedByUserId))
                throw new BadRequestException("Assigned By User does not exist.");

            if (dto.ReturnedDate.HasValue &&
                dto.ReturnedDate < dto.AssignedDate)
                throw new BadRequestException("Returned date cannot be before assigned date.");

            if (await _licenseAssignmentRepository.ActiveAssignmentExistsAsync(dto.LicenseId, dto.EmployeeId))
                throw new ConflictException("This license is already assigned to this employee.");

            var assignment = new LicenseAssignment
            {
                LicenseId = dto.LicenseId,
                EmployeeId = dto.EmployeeId,
                AssignedDate = dto.AssignedDate,
                ReturnedDate = dto.ReturnedDate,
                AssignmentStatus = dto.AssignmentStatus,
                AssignedByUserId = dto.AssignedByUserId,
                Remarks = dto.Remarks?.Trim()
            };

            await _licenseAssignmentRepository.AddAsync(assignment);
            await _licenseAssignmentRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "CREATE",
                "LicenseAssignment",
                assignment.AssignmentId,
                $"Assigned license '{assignment.LicenseId}' to employee '{assignment.EmployeeId}'"
            );

            var created = await _licenseAssignmentRepository.GetByIdAsync(assignment.AssignmentId);

            return new LicenseAssignmentDto
            {
                AssignmentId = created!.AssignmentId,
                LicenseId = created.LicenseId,
                LicenseKey = created.License.LicenseKey,
                EmployeeId = created.EmployeeId,
                EmployeeName = $"{created.Employee.FirstName} {created.Employee.LastName}",
                AssignedDate = created.AssignedDate,
                ReturnedDate = created.ReturnedDate,
                AssignmentStatus = created.AssignmentStatus,
                AssignedByUserId = created.AssignedByUserId,
                AssignedByUsername = created.AssignedByUser.Username,
                Remarks = created.Remarks
            };
        }

        public async Task<bool> UpdateAsync(int assignmentId, UpdateLicenseAssignmentDto dto)
        {
            var assignment = await _licenseAssignmentRepository.GetByIdAsync(assignmentId);

            if (assignment == null)
                return false;

            if (!await _licenseAssignmentRepository.LicenseExistsAsync(dto.LicenseId))
                throw new BadRequestException("License does not exist.");

            if (!await _licenseAssignmentRepository.EmployeeExistsAsync(dto.EmployeeId))
                throw new BadRequestException("Employee does not exist.");

            if (!await _licenseAssignmentRepository.UserExistsAsync(dto.AssignedByUserId))
                throw new BadRequestException("Assigned By User does not exist.");

            if (dto.ReturnedDate.HasValue &&
                dto.ReturnedDate < dto.AssignedDate)
                throw new BadRequestException("Returned date cannot be before assigned date.");

            bool isDuplicate =
                assignment.LicenseId != dto.LicenseId ||
                assignment.EmployeeId != dto.EmployeeId;

            if (isDuplicate &&
                await _licenseAssignmentRepository.ActiveAssignmentExistsAsync(dto.LicenseId, dto.EmployeeId))
            {
                throw new ConflictException("This license is already assigned to this employee.");
            }

            assignment.LicenseId = dto.LicenseId;
            assignment.EmployeeId = dto.EmployeeId;
            assignment.AssignedDate = dto.AssignedDate;
            assignment.ReturnedDate = dto.ReturnedDate;
            assignment.AssignmentStatus = dto.AssignmentStatus;
            assignment.AssignedByUserId = dto.AssignedByUserId;
            assignment.Remarks = dto.Remarks?.Trim();

            await _licenseAssignmentRepository.UpdateAsync(assignment);
            await _licenseAssignmentRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "UPDATE",
                "LicenseAssignment",
                assignment.AssignmentId,
                $"Updated assignment #{assignment.AssignmentId}"
            );

            return true;
        }

        public async Task<bool> DeleteAsync(int assignmentId)
        {
            var assignment = await _licenseAssignmentRepository.GetByIdAsync(assignmentId);

            if (assignment == null)
                return false;

            int id = assignment.AssignmentId;

            await _licenseAssignmentRepository.DeleteAsync(assignment);
            await _licenseAssignmentRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "DELETE",
                "LicenseAssignment",
                id,
                $"Deleted assignment #{id}"
            );

            return true;
        }
    }
}