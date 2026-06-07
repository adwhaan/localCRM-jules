using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Engagements.Queries;

public record SearchEngagementsQuery(string Term, int Offset = 0, int Limit = 10) : IRequest<PagedResult<EngagementDto>>;

public class SearchEngagementsQueryHandler : IRequestHandler<SearchEngagementsQuery, PagedResult<EngagementDto>>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IMapper _mapper;

    public SearchEngagementsQueryHandler(IRepository<Engagement> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<EngagementDto>> Handle(SearchEngagementsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query().Where(e => !e.IsDeleted);
        // Simple search implementation
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync(cancellationToken);

        return new PagedResult<EngagementDto> { Items = _mapper.Map<List<EngagementDto>>(items), TotalCount = totalCount, Offset = request.Offset, Limit = request.Limit };
    }
}
