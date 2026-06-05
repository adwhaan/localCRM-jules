using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Documents.Commands;

public record SoftDeleteDocumentCommand(int Id) : IRequest;

public class SoftDeleteDocumentCommandHandler : IRequestHandler<SoftDeleteDocumentCommand>
{
    private readonly IRepository<Document> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public SoftDeleteDocumentCommandHandler(IRepository<Document> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(SoftDeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || entity.IsDeleted) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Documents", entity.DocumentId, "SOFT_DELETE", entity.UpdatedBy);
    }
}
