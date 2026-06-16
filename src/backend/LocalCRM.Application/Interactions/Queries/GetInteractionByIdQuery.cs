using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Interactions.Queries;

public record GetInteractionByIdQuery(int Id) : IRequest<InteractionDto?>;

public class GetInteractionByIdQueryHandler : IRequestHandler<GetInteractionByIdQuery, InteractionDto?>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IMapper _mapper;

    public GetInteractionByIdQueryHandler(IRepository<Interaction> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<InteractionDto?> Handle(GetInteractionByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        return entity == null || entity.IsDeleted ? null : _mapper.Map<InteractionDto>(entity);
    }
}
