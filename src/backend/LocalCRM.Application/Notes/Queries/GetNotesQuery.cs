using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Notes.Queries;

public record GetNotesQuery : IRequest<List<NoteDto>>;

public class GetNotesQueryHandler : IRequestHandler<GetNotesQuery, List<NoteDto>>
{
    private readonly IRepository<Note> _repository;
    private readonly IMapper _mapper;

    public GetNotesQueryHandler(IRepository<Note> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<NoteDto>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<List<NoteDto>>(entities.Where(e => !e.IsDeleted).ToList());
    }
}
