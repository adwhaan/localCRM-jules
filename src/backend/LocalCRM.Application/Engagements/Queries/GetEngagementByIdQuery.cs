using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Engagements.Queries;

public record GetEngagementByIdQuery(int Id) : IRequest<EngagementDto?>;

public class GetEngagementByIdQueryHandler : IRequestHandler<GetEngagementByIdQuery, EngagementDto?>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IMapper _mapper;

    public GetEngagementByIdQueryHandler(IRepository<Engagement> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EngagementDto?> Handle(GetEngagementByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        return entity == null || entity.IsDeleted ? null : _mapper.Map<EngagementDto>(entity);
    }
}
