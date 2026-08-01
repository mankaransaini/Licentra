using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Models;

[Index("RoleName", Name = "UQ__Roles__8A2B6160B6F456A6", IsUnique = true)]
public partial class Role
{
    [Key]
    public int RoleId { get; set; }

    [StringLength(50)]
    public string RoleName { get; set; } = null!;

    [StringLength(200)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
