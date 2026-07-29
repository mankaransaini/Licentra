using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Models;

[Index("EmployeeId", Name = "IX_LicenseAssignments_EmployeeId")]
public partial class LicenseAssignment
{
    [Key]
    public int AssignmentId { get; set; }

    [Column("LicenseID")]
    public int LicenseId { get; set; }

    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime AssignedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReturnedDate { get; set; }

    public byte AssignmentStatus { get; set; }

    public int AssignedByUserId { get; set; }

    [StringLength(300)]
    public string? Remarks { get; set; }

    [ForeignKey("AssignedByUserId")]
    [InverseProperty("LicenseAssignments")]
    public virtual User AssignedByUser { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("LicenseAssignments")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("LicenseId")]
    [InverseProperty("LicenseAssignments")]
    public virtual License License { get; set; } = null!;
}
