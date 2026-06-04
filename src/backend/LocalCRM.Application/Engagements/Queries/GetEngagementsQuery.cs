using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Engagements.Queries;

public record GetEngagementsQuery : IRequest<List<EngagementDto>>;

public class GetEngagementsQueryHandler : IRequestHandler<GetEngagementsQuery, List<EngagementDto>>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IMapper _mapper;

    public GetEngagementsQueryHandler(IRepository<Engagement> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<EngagementDto>> Handle(GetEngagementsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<List<EngagementDto>>(entities.Where(e => !e.IsDeleted).ToList());
    }
}
