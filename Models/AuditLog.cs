using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Licentra.API.Models;

public partial class AuditLog
{
    [Key]
    public int AuditLogId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(100)]
    public string Action { get; set; } = null!;

    [StringLength(50)]
    public string TableName { get; set; } = null!;

    public int RecordId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ActionDate { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AuditLogs")]
    public virtual User User { get; set; } = null!;
}
