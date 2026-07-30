using Licentra.API.DTOs.Employees;
using Licentra.API.Exceptions.Custom;
using Licentra.API.Interfaces.AuditLogs;
using Licentra.API.Interfaces.Employees;
using Licentra.API.Models;

namespace Licentra.API.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAuditLogService _auditLogService;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IAuditLogService auditLogService)
        {
            _employeeRepository = employeeRepository ??
                throw new ArgumentNullException(nameof(employeeRepository));

            _auditLogService = auditLogService ??
                throw new ArgumentNullException(nameof(auditLogService));
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            return employees.Select(e => new EmployeeDto
            {
                EmployeeId = e.EmployeeId,
                EmployeeCode = e.EmployeeCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.DepartmentName,
                Designation = e.Designation,
                JoiningDate = e.JoiningDate,
                EmploymentStatus = e.EmploymentStatus,
                IsActive = e.IsActive ?? true
            });
        }

        public async Task<EmployeeDto?> GetByIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                return null;

            return new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department.DepartmentName,
                Designation = employee.Designation,
                JoiningDate = employee.JoiningDate,
                EmploymentStatus = employee.EmploymentStatus,
                IsActive = employee.IsActive ?? true
            };
        }

        public async Task<EmployeeDto> AddAsync(CreateEmployeeDto dto)
        {
            if (await _employeeRepository.EmailExistsAsync(dto.Email))
            {
                throw new ConflictException("Email already exists.");
            }

            if (!await _employeeRepository.DepartmentExistsAsync(dto.DepartmentId))
            {
                throw new BadRequestException("Invalid Department.");
            }

            if (await _employeeRepository.EmployeeCodeExistsAsync(dto.EmployeeCode))
            {
                throw new ConflictException("Employee code already exists.");
            }

            var employee = new Employee
            {
                EmployeeCode = dto.EmployeeCode.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                Phone = dto.Phone?.Trim(),
                DepartmentId = dto.DepartmentId,
                Designation = dto.Designation.Trim(),
                JoiningDate = dto.JoiningDate,
                EmploymentStatus = dto.EmploymentStatus,
                IsActive = true
            };

            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "CREATE",
                "Employee",
                employee.EmployeeId,
                $"Created employee '{employee.FirstName} {employee.LastName}'"
            );

            employee = await _employeeRepository.GetByIdAsync(employee.EmployeeId)
                       ?? throw new Exception("Employee could not be loaded after creation.");

            return new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department.DepartmentName,
                Designation = employee.Designation,
                JoiningDate = employee.JoiningDate,
                EmploymentStatus = employee.EmploymentStatus,
                IsActive = employee.IsActive ?? true
            };
        }

        public async Task<bool> UpdateAsync(int employeeId, UpdateEmployeeDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                return false;

            if (!await _employeeRepository.DepartmentExistsAsync(dto.DepartmentId))
            {
                throw new BadRequestException("Invalid Department.");
            }

            employee.FirstName = dto.FirstName.Trim();
            employee.LastName = dto.LastName.Trim();
            employee.Email = dto.Email.Trim();
            employee.Phone = dto.Phone?.Trim();
            employee.DepartmentId = dto.DepartmentId;
            employee.Designation = dto.Designation.Trim();
            employee.JoiningDate = dto.JoiningDate;
            employee.EmploymentStatus = dto.EmploymentStatus;
            employee.IsActive = dto.IsActive;

            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "UPDATE",
                "Employee",
                employee.EmployeeId,
                $"Updated employee '{employee.FirstName} {employee.LastName}'"
            );

            return true;
        }

        public async Task<bool> DeleteAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                return false;

            string employeeName = $"{employee.FirstName} {employee.LastName}";

            await _employeeRepository.DeleteAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "DELETE",
                "Employee",
                employee.EmployeeId,
                $"Deleted employee '{employeeName}'"
            );

            return true;
        }
    }
}