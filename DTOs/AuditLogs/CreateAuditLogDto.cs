namespace Licentra.API.DTOs.AuditLogs
{
    public class CreateAuditLogDto
    {
        public int UserId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string TableName { get; set; } = string.Empty;

        public int RecordId { get; set; }

        public string? Description { get; set; }
    }
}