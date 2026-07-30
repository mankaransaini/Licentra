using Licentra.API.DTOs.Departments;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Interfaces.Departments;
using Licentra.API.Models;
using Licentra.API.Exceptions.Custom;

namespace Licentra.API.Services.Departments
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAuditLogService _auditLogService;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IAuditLogService auditLogService)
        {
            _departmentRepository = departmentRepository ??
                throw new ArgumentNullException(nameof(departmentRepository));

            _auditLogService = auditLogService ??
                throw new ArgumentNullException(nameof(auditLogService));
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();

            return departments.Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName,
                Description = d.Description,
                IsActive = d.IsActive
            });
        }

        public async Task<DepartmentDto?> GetByIdAsync(int departmentId)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);

            if (department == null)
                return null;

            return new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                Description = department.Description,
                IsActive = department.IsActive
            };
        }

        public async Task<DepartmentDto> AddAsync(CreateDepartmentDto createDepartmentDto)
        {
            var existingDepartment = await _departmentRepository.GetByNameAsync(createDepartmentDto.DepartmentName);

            if (existingDepartment != null)
            {
                throw new ConflictException("Department already exists.");
            }

            var newDepartment = new Department
            {
                DepartmentName = createDepartmentDto.DepartmentName.Trim(),
                Description = createDepartmentDto.Description?.Trim(),
                IsActive = true
            };

            await _departmentRepository.AddAsync(newDepartment);
            await _departmentRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "CREATE",
                "Department",
                newDepartment.DepartmentId,
                $"Created department '{newDepartment.DepartmentName}'"
            );

            return new DepartmentDto
            {
                DepartmentId = newDepartment.DepartmentId,
                DepartmentName = newDepartment.DepartmentName,
                Description = newDepartment.Description,
                IsActive = newDepartment.IsActive
            };
        }

        public async Task<bool> UpdateAsync(int departmentId, UpdateDepartmentDto updateDepartmentDto)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);

            if (department == null)
                return false;

            department.DepartmentName = updateDepartmentDto.DepartmentName;
            department.Description = updateDepartmentDto.Description;
            department.IsActive = updateDepartmentDto.IsActive;

            await _departmentRepository.UpdateAsync(department);
            await _departmentRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "UPDATE",
                "Department",
                department.DepartmentId,
                $"Updated department '{department.DepartmentName}'"
            );

            return true;
        }

        public async Task<bool> DeleteAsync(int departmentId)
        {
            var department = await _departmentRepository.GetByIdAsync(departmentId);

            if (department == null)
                return false;

            string departmentName = department.DepartmentName;

            await _departmentRepository.DeleteAsync(department);
            await _departmentRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "DELETE",
                "Department",
                department.DepartmentId,
                $"Deleted department '{departmentName}'"
            );

            return true;
        }
    }
}