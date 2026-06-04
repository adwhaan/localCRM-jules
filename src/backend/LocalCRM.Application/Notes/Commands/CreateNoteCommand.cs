using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Notes.Commands;

public record CreateNoteCommand : IRequest<NoteDto>
{
    public string Subject { get; init; } = string.Empty;
    public string? Body { get; init; }
    public string Visibility { get; init; } = string.Empty;
}

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, NoteDto>
{
    private readonly IRepository<Note> _repository;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public CreateNoteCommandHandler(IRepository<Note> repository, IMapper mapper, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<NoteDto> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var entity = new Note
        {
            Subject = request.Subject,
            Body = request.Body,
            Visibility = request.Visibility,
            CreatedBy = _currentUser.Username ?? "system"
        };

        await _repository.AddAsync(entity);
        await _audit.LogAsync("notes", entity.NoteId, "CREATE", entity.CreatedBy);

        return _mapper.Map<NoteDto>(entity);
    }
}
