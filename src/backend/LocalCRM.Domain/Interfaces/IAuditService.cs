using System.Threading.Tasks;

namespace LocalCRM.Domain.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string entityName, int entityId, string actionType, string? notes = null, string? performedBy = null);
    }
}
