using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Contacts.Commands;

public record BulkDeleteContactsCommand(List<int> Ids) : IRequest;

public class BulkDeleteContactsCommandHandler : IRequestHandler<BulkDeleteContactsCommand>
{
    private readonly IRepository<Contact> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public BulkDeleteContactsCommandHandler(IRepository<Contact> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(BulkDeleteContactsCommand request, CancellationToken cancellationToken)
    {
        var entities = await _repository.Query().Where(e => request.Ids.Contains(e.ContactId) && !e.IsDeleted).ToListAsync(cancellationToken);
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.Username ?? "system";
            await _audit.LogAsync("Contacts", entity.ContactId, "SOFT_DELETE", entity.UpdatedBy, "Bulk");
        }
        await _repository.BulkUpdateAsync(entities);
    }
}
