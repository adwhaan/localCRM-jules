using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Contacts.Commands;

public record SoftDeleteContactCommand(int Id) : IRequest;

public class SoftDeleteContactCommandHandler : IRequestHandler<SoftDeleteContactCommand>
{
    private readonly IRepository<Contact> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public SoftDeleteContactCommandHandler(IRepository<Contact> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(SoftDeleteContactCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || entity.IsDeleted) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Contacts", entity.ContactId, "SOFT_DELETE", entity.UpdatedBy);
    }
}
