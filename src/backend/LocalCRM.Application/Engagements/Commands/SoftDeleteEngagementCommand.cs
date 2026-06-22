using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Engagements.Commands;

public record SoftDeleteEngagementCommand(int Id) : IRequest;

public class SoftDeleteEngagementCommandHandler : IRequestHandler<SoftDeleteEngagementCommand>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public SoftDeleteEngagementCommandHandler(IRepository<Engagement> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(SoftDeleteEngagementCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || entity.IsDeleted) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Engagements", entity.EngagementId, "SOFT_DELETE", entity.UpdatedBy);
    }
}
