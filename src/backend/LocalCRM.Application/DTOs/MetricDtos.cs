using System;
using System.Collections.Generic;

namespace LocalCRM.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalCompanies { get; set; }
        public int TotalContacts { get; set; }
        public int TotalInteractions { get; set; }
        public int UpcomingTasks { get; set; }
        public List<RecentInteractionDto> RecentInteractions { get; set; } = new();
    }

    public class RecentInteractionDto
    {
        public int InteractionId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public DateTime InteractionDate { get; set; }
        public string? ContactName { get; set; }
    }

    public class SystemMetricsDto
    {
        public DateTime StartupTime { get; set; }
        public string Uptime { get; set; } = string.Empty;
        public long DatabaseSize { get; set; }
        public int TotalApiCalls { get; set; }
        public int TotalApiFailures { get; set; }
        public DateTime? LastApiAction { get; set; }
    }
}
