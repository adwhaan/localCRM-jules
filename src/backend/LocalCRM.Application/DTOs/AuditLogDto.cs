using System;

namespace LocalCRM.Application.DTOs
{
    public class AuditLogDto
    {
        public int AuditId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public DateTime PerformedAt { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
