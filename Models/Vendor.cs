using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Models;

[Index("VendorName", Name = "UQ__Vendor__7320A357B0CFB081", IsUnique = true)]
public partial class Vendor
{
    [Key]
    public int VendorId { get; set; }

    [StringLength(100)]
    public string VendorName { get; set; } = null!;

    [StringLength(100)]
    public string? ContactPerson { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Website { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    public bool? IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Vendor")]
    public virtual ICollection<Software> Softwares { get; set; } = new List<Software>();
}
