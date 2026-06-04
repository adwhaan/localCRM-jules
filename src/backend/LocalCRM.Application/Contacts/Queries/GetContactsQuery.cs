using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Contacts.Queries;

public record GetContactsQuery : IRequest<List<ContactDto>>;

public class GetContactsQueryHandler : IRequestHandler<GetContactsQuery, List<ContactDto>>
{
    private readonly IRepository<Contact> _repository;
    private readonly IMapper _mapper;

    public GetContactsQueryHandler(IRepository<Contact> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ContactDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<List<ContactDto>>(entities.Where(c => !c.IsDeleted).ToList());
    }
}
