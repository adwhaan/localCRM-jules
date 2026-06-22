using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Engagements.Commands;

public record BulkDeleteEngagementsCommand(List<int> Ids) : IRequest;

public class BulkDeleteEngagementsCommandHandler : IRequestHandler<BulkDeleteEngagementsCommand>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public BulkDeleteEngagementsCommandHandler(IRepository<Engagement> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(BulkDeleteEngagementsCommand request, CancellationToken cancellationToken)
    {
        var entities = await _repository.Query().Where(e => request.Ids.Contains(e.EngagementId) && !e.IsDeleted).ToListAsync(cancellationToken);
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.Username ?? "system";
            await _audit.LogAsync("Engagements", entity.EngagementId, "SOFT_DELETE", entity.UpdatedBy, "Bulk");
        }
        await _repository.BulkUpdateAsync(entities);
    }
}
