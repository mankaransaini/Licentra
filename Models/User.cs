using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Models;

[Index("Username", Name = "UQ__Users__536C85E4917311F3", IsUnique = true)]
[Index("EmployeeId", Name = "UQ__Users__7AD04FF082ECC050", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D1053414D224A3", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column("RoleID")]
    public int RoleId { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? LastLogin { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [ForeignKey("EmployeeId")]
    [InverseProperty("User")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("AssignedByUser")]
    public virtual ICollection<LicenseAssignment> LicenseAssignments { get; set; } = new List<LicenseAssignment>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;
}
