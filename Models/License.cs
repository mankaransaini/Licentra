using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Models;

[Index("LicenseStatus", Name = "IX_Licenses_Status")]
public partial class License
{
    [Key]
    public int LicenseId { get; set; }

    [Column("SoftwareID")]
    public int SoftwareId { get; set; }

    [StringLength(255)]
    public string LicenseKey { get; set; } = null!;

    [StringLength(30)]
    public string LicenseType { get; set; } = null!;

    public DateOnly PurchaseDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PurchaseCost { get; set; }

    public int Seats { get; set; }

    public byte LicenseStatus { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("License")]
    public virtual ICollection<LicenseAssignment> LicenseAssignments { get; set; } = new List<LicenseAssignment>();

    [ForeignKey("SoftwareId")]
    [InverseProperty("Licenses")]
    public virtual Software Software { get; set; } = null!;
}
