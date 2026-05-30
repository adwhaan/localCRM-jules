using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Interfaces;
using LocalCRM.Infrastructure.Persistence;
using System.Threading.Tasks;

namespace LocalCRM.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly LocalCRMContext _context;

        public AuditService(LocalCRMContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string entityName, int entityId, string actionType, string? notes = null, string? performedBy = null)
        {
            var auditLog = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                ActionType = actionType,
                Notes = notes,
                PerformedBy = performedBy ?? "system",
                PerformedAt = System.DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}
