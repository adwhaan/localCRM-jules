using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Contacts.Commands;

public record BulkRestoreContactsCommand(List<int> Ids) : IRequest;

public class BulkRestoreContactsCommandHandler : IRequestHandler<BulkRestoreContactsCommand>
{
    private readonly IRepository<Contact> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public BulkRestoreContactsCommandHandler(IRepository<Contact> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(BulkRestoreContactsCommand request, CancellationToken cancellationToken)
    {
        var entities = await _repository.Query().Where(e => request.Ids.Contains(e.ContactId) && e.IsDeleted).ToListAsync(cancellationToken);
        foreach (var entity in entities)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.UpdatedBy = _currentUser.Username ?? "system";
            await _audit.LogAsync("Contacts", entity.ContactId, "RESTORE", entity.UpdatedBy, "Bulk");
        }
        await _repository.BulkUpdateAsync(entities);
    }
}
