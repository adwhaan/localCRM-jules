using System.Threading.Tasks;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Infrastructure.Persistence;

namespace LocalCRM.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string entityName, int entityId, string actionType, string performedBy, string? notes = null)
    {
        var log = new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            ActionType = actionType,
            PerformedBy = performedBy,
            Notes = notes,
            PerformedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
