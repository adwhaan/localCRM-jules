using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Interactions.Queries;

public record GetInteractionsQuery : IRequest<List<InteractionDto>>;

public class GetInteractionsQueryHandler : IRequestHandler<GetInteractionsQuery, List<InteractionDto>>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IMapper _mapper;

    public GetInteractionsQueryHandler(IRepository<Interaction> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<InteractionDto>> Handle(GetInteractionsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<List<InteractionDto>>(entities.Where(c => !c.IsDeleted).ToList());
    }
}
