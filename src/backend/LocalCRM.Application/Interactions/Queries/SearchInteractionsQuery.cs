using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Interactions.Queries;

public record SearchInteractionsQuery(string Term, int Offset = 0, int Limit = 10) : IRequest<PagedResult<InteractionDto>>;

public class SearchInteractionsQueryHandler : IRequestHandler<SearchInteractionsQuery, PagedResult<InteractionDto>>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IMapper _mapper;

    public SearchInteractionsQueryHandler(IRepository<Interaction> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<InteractionDto>> Handle(SearchInteractionsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query().Where(e => !e.IsDeleted);
        // Simple search implementation
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync(cancellationToken);

        return new PagedResult<InteractionDto> { Items = _mapper.Map<List<InteractionDto>>(items), TotalCount = totalCount, Offset = request.Offset, Limit = request.Limit };
    }
}
