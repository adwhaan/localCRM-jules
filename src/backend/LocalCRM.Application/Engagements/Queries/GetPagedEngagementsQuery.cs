using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Engagements.Queries;

public record GetPagedEngagementsQuery(int Offset = 0, int Limit = 10, bool IncludeDeleted = false) : IRequest<PagedResult<EngagementDto>>;

public class GetPagedEngagementsQueryHandler : IRequestHandler<GetPagedEngagementsQuery, PagedResult<EngagementDto>>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IMapper _mapper;

    public GetPagedEngagementsQueryHandler(IRepository<Engagement> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<EngagementDto>> Handle(GetPagedEngagementsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query();
        query = request.IncludeDeleted ? query.Where(e => e.IsDeleted) : query.Where(e => !e.IsDeleted);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync(cancellationToken);
        return new PagedResult<EngagementDto> { Items = _mapper.Map<List<EngagementDto>>(items), TotalCount = totalCount, Offset = request.Offset, Limit = request.Limit };
    }
}
