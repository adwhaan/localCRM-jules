using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Notes.Commands;

public record SoftDeleteNoteCommand(int Id) : IRequest;

public class SoftDeleteNoteCommandHandler : IRequestHandler<SoftDeleteNoteCommand>
{
    private readonly IRepository<Note> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public SoftDeleteNoteCommandHandler(IRepository<Note> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(SoftDeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || entity.IsDeleted) return;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Notes", entity.NoteId, "SOFT_DELETE", entity.UpdatedBy);
    }
}
