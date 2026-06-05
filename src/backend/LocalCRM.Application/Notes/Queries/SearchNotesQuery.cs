using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Notes.Queries;

public record SearchNotesQuery(string Term, int Offset = 0, int Limit = 10) : IRequest<PagedResult<NoteDto>>;

public class SearchNotesQueryHandler : IRequestHandler<SearchNotesQuery, PagedResult<NoteDto>>
{
    private readonly IRepository<Note> _repository;
    private readonly IMapper _mapper;

    public SearchNotesQueryHandler(IRepository<Note> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<NoteDto>> Handle(SearchNotesQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query().Where(e => !e.IsDeleted);
        // Simple search implementation
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync(cancellationToken);

        return new PagedResult<NoteDto> { Items = _mapper.Map<List<NoteDto>>(items), TotalCount = totalCount, Offset = request.Offset, Limit = request.Limit };
    }
}
