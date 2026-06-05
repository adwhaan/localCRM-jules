using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Documents.Commands;

public record BulkDeleteDocumentsCommand(List<int> Ids) : IRequest;

public class BulkDeleteDocumentsCommandHandler : IRequestHandler<BulkDeleteDocumentsCommand>
{
    private readonly IRepository<Document> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public BulkDeleteDocumentsCommandHandler(IRepository<Document> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(BulkDeleteDocumentsCommand request, CancellationToken cancellationToken)
    {
        var entities = await _repository.Query().Where(e => request.Ids.Contains(e.DocumentId) && !e.IsDeleted).ToListAsync(cancellationToken);
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.Username ?? "system";
            await _audit.LogAsync("Documents", entity.DocumentId, "SOFT_DELETE", entity.UpdatedBy, "Bulk");
        }
        await _repository.BulkUpdateAsync(entities);
    }
}
