using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Documents.Queries;

public record GetPagedDocumentsQuery(int Offset = 0, int Limit = 10, bool IncludeDeleted = false) : IRequest<PagedResult<DocumentDto>>;

public class GetPagedDocumentsQueryHandler : IRequestHandler<GetPagedDocumentsQuery, PagedResult<DocumentDto>>
{
    private readonly IRepository<Document> _repository;
    private readonly IMapper _mapper;

    public GetPagedDocumentsQueryHandler(IRepository<Document> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<DocumentDto>> Handle(GetPagedDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query();
        query = request.IncludeDeleted ? query.Where(e => e.IsDeleted) : query.Where(e => !e.IsDeleted);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync(cancellationToken);
        return new PagedResult<DocumentDto> { Items = _mapper.Map<List<DocumentDto>>(items), TotalCount = totalCount, Offset = request.Offset, Limit = request.Limit };
    }
}
