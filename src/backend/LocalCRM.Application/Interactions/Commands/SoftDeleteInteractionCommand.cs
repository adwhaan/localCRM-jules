using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Interactions.Commands;

public record SoftDeleteInteractionCommand(int Id) : IRequest;

public class SoftDeleteInteractionCommandHandler : IRequestHandler<SoftDeleteInteractionCommand>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public SoftDeleteInteractionCommandHandler(IRepository<Interaction> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(SoftDeleteInteractionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || entity.IsDeleted) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Interactions", entity.InteractionId, "SOFT_DELETE", entity.UpdatedBy);
    }
}
