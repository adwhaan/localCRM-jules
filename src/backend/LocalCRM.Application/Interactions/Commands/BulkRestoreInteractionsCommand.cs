using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Interactions.Commands;

public record BulkRestoreInteractionsCommand(List<int> Ids) : IRequest;

public class BulkRestoreInteractionsCommandHandler : IRequestHandler<BulkRestoreInteractionsCommand>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public BulkRestoreInteractionsCommandHandler(IRepository<Interaction> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(BulkRestoreInteractionsCommand request, CancellationToken cancellationToken)
    {
        var entities = await _repository.Query().Where(e => request.Ids.Contains(e.InteractionId) && e.IsDeleted).ToListAsync(cancellationToken);
        foreach (var entity in entities)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.UpdatedBy = _currentUser.Username ?? "system";
            await _audit.LogAsync("Interactions", entity.InteractionId, "RESTORE", entity.UpdatedBy, "Bulk");
        }
        await _repository.BulkUpdateAsync(entities);
    }
}
