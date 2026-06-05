using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Notes.Queries;

public record GetPagedNotesQuery(int Offset = 0, int Limit = 10, bool IncludeDeleted = false) : IRequest<PagedResult<NoteDto>>;

public class GetPagedNotesQueryHandler : IRequestHandler<GetPagedNotesQuery, PagedResult<NoteDto>>
{
    private readonly IRepository<Note> _repository;
    private readonly IMapper _mapper;

    public GetPagedNotesQueryHandler(IRepository<Note> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<NoteDto>> Handle(GetPagedNotesQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query();
        query = request.IncludeDeleted ? query.Where(e => e.IsDeleted) : query.Where(e => !e.IsDeleted);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync(cancellationToken);
        return new PagedResult<NoteDto> { Items = _mapper.Map<List<NoteDto>>(items), TotalCount = totalCount, Offset = request.Offset, Limit = request.Limit };
    }
}
