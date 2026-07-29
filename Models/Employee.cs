using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Models;

[Index("DepartmentId", Name = "IX_Employees_DepartmentId")]
[Index("FirstName", "LastName", Name = "IX_Employees_Name")]
[Index("EmployeeCode", Name = "UQ__Employee__1F6425486B546F5C", IsUnique = true)]
[Index("Email", Name = "UQ__Employee__A9D10534970D1DEE", IsUnique = true)]
public partial class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    [StringLength(20)]
    public string EmployeeCode { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    public int DepartmentId { get; set; }

    [StringLength(100)]
    public string Designation { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Phone { get; set; }

    public DateOnly JoiningDate { get; set; }

    [StringLength(20)]
    public string? EmploymentStatus { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("DepartmentId")]
    [InverseProperty("Employees")]
    public virtual Department Department { get; set; } = null!;

    [InverseProperty("Employee")]
    public virtual ICollection<LicenseAssignment> LicenseAssignments { get; set; } = new List<LicenseAssignment>();

    [InverseProperty("Employee")]
    public virtual User? User { get; set; }
}
