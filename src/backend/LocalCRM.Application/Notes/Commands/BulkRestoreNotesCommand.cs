using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Notes.Commands;

public record BulkRestoreNotesCommand(List<int> Ids) : IRequest;

public class BulkRestoreNotesCommandHandler : IRequestHandler<BulkRestoreNotesCommand>
{
    private readonly IRepository<Note> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public BulkRestoreNotesCommandHandler(IRepository<Note> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(BulkRestoreNotesCommand request, CancellationToken cancellationToken)
    {
        var entities = await _repository.Query().Where(e => request.Ids.Contains(e.NoteId) && e.IsDeleted).ToListAsync(cancellationToken);
        foreach (var entity in entities)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.UpdatedBy = _currentUser.Username ?? "system";
            await _audit.LogAsync("Notes", entity.NoteId, "RESTORE", entity.UpdatedBy, "Bulk");
        }
        await _repository.BulkUpdateAsync(entities);
    }
}
