using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Notes.Queries;

public record GetNoteByIdQuery(int Id) : IRequest<NoteDto?>;

public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, NoteDto?>
{
    private readonly IRepository<Note> _repository;
    private readonly IMapper _mapper;

    public GetNoteByIdQueryHandler(IRepository<Note> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<NoteDto?> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        return entity == null || entity.IsDeleted ? null : _mapper.Map<NoteDto>(entity);
    }
}
