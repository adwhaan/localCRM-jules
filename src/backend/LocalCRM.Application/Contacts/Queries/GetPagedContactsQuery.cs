using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Contacts.Queries;

public record GetPagedContactsQuery(int Offset = 0, int Limit = 10, bool IncludeDeleted = false) : IRequest<PagedResult<ContactDto>>;

public class GetPagedContactsQueryHandler : IRequestHandler<GetPagedContactsQuery, PagedResult<ContactDto>>
{
    private readonly IRepository<Contact> _repository;
    private readonly IMapper _mapper;

    public GetPagedContactsQueryHandler(IRepository<Contact> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ContactDto>> Handle(GetPagedContactsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query();
        query = request.IncludeDeleted ? query.Where(e => e.IsDeleted) : query.Where(e => !e.IsDeleted);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync(cancellationToken);
        return new PagedResult<ContactDto> { Items = _mapper.Map<List<ContactDto>>(items), TotalCount = totalCount, Offset = request.Offset, Limit = request.Limit };
    }
}
