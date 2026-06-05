using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Documents.Commands;

public record BulkRestoreDocumentsCommand(List<int> Ids) : IRequest;

public class BulkRestoreDocumentsCommandHandler : IRequestHandler<BulkRestoreDocumentsCommand>
{
    private readonly IRepository<Document> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public BulkRestoreDocumentsCommandHandler(IRepository<Document> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(BulkRestoreDocumentsCommand request, CancellationToken cancellationToken)
    {
        var entities = await _repository.Query().Where(e => request.Ids.Contains(e.DocumentId) && e.IsDeleted).ToListAsync(cancellationToken);
        foreach (var entity in entities)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.UpdatedBy = _currentUser.Username ?? "system";
            await _audit.LogAsync("Documents", entity.DocumentId, "RESTORE", entity.UpdatedBy, "Bulk");
        }
        await _repository.BulkUpdateAsync(entities);
    }
}
