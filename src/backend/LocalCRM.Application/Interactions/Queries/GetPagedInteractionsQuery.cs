using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Interactions.Queries;

public record GetPagedInteractionsQuery(int Offset = 0, int Limit = 10, bool IncludeDeleted = false) : IRequest<PagedResult<InteractionDto>>;

public class GetPagedInteractionsQueryHandler : IRequestHandler<GetPagedInteractionsQuery, PagedResult<InteractionDto>>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IMapper _mapper;

    public GetPagedInteractionsQueryHandler(IRepository<Interaction> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<InteractionDto>> Handle(GetPagedInteractionsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query();
        query = request.IncludeDeleted ? query.Where(i => i.IsDeleted) : query.Where(i => !i.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(i => i.InteractionDate)
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
