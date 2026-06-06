using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Documents.Queries;

public record SearchDocumentsQuery(string Term, int Offset = 0, int Limit = 10) : IRequest<PagedResult<DocumentDto>>;

public class SearchDocumentsQueryHandler : IRequestHandler<SearchDocumentsQuery, PagedResult<DocumentDto>>
{
    private readonly IRepository<Document> _repository;
    private readonly IMapper _mapper;

    public SearchDocumentsQueryHandler(IRepository<Document> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<DocumentDto>> Handle(SearchDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query().Where(e => !e.IsDeleted);
        // Simple search implementation
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync(cancellationToken);

        return new PagedResult<DocumentDto> { Items = _mapper.Map<List<DocumentDto>>(items), TotalCount = totalCount, Offset = request.Offset, Limit = request.Limit };
    }
}
