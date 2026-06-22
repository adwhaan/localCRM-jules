using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(string entityName, int entityId, string actionType, string performedBy, string? notes = null);
}
