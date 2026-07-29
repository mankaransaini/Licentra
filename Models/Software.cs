using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Models;

[Table("Software")]
[Index("SoftwareName", "Version", Name = "UQ_Software_SoftwareName_Version", IsUnique = true)]
public partial class Software
{
    [Key]
    public int SoftwareId { get; set; }

    [Column("VendorID")]
    public int VendorId { get; set; }

    [StringLength(100)]
    public string SoftwareName { get; set; } = null!;

    [StringLength(30)]
    public string Version { get; set; } = null!;

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public bool? IsSubscription { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Software")]
    public virtual ICollection<License> Licenses { get; set; } = new List<License>();

    [ForeignKey("VendorId")]
    [InverseProperty("Softwares")]
    public virtual Vendor Vendor { get; set; } = null!;
}
