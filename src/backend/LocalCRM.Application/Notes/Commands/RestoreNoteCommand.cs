using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using AutoMapper;

namespace LocalCRM.Application.Notes.Commands;

public record RestoreNoteCommand(int Id) : IRequest<NoteDto>;

public class RestoreNoteCommandHandler : IRequestHandler<RestoreNoteCommand, NoteDto>
{
    private readonly IRepository<Note> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public RestoreNoteCommandHandler(IRepository<Note> repository, IAuditService audit, ICurrentUserService currentUser, IMapper mapper)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<NoteDto> Handle(RestoreNoteCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || !entity.IsDeleted) throw new Exception("Not found or not deleted");
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Notes", entity.NoteId, "RESTORE", entity.UpdatedBy);
        return _mapper.Map<NoteDto>(entity);
    }
}
