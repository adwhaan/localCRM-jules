using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Interactions.Queries;

public record GetInteractionsQuery(int Offset = 0, int Limit = 10, bool IncludeDeleted = false) : IRequest<PagedResult<InteractionDto>>;

public class GetInteractionsQueryHandler : IRequestHandler<GetInteractionsQuery, PagedResult<InteractionDto>>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IMapper _mapper;

    public GetInteractionsQueryHandler(IRepository<Interaction> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<InteractionDto>> Handle(GetInteractionsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query();
        query = request.IncludeDeleted ? query.Where(i => i.IsDeleted) : query.Where(i => !i.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(i => i.InteractionDate).ThenByDescending(i => i.InteractionTime)
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return new PagedResult<InteractionDto>
        {
            Items = _mapper.Map<List<InteractionDto>>(items),
            TotalCount = totalCount,
            Offset = request.Offset,
            Limit = request.Limit
        };
    }
}
