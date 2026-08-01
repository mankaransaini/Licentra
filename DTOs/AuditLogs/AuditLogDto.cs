namespace Licentra.API.DTOs.AuditLogs
{
    public class AuditLogDto
    {
        public int AuditLogId { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string TableName { get; set; } = string.Empty;

        public int RecordId { get; set; }

        public string? Description { get; set; }

        public DateTime? ActionDate { get; set; }
    }
}