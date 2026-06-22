using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Contacts.Queries;

public record SearchContactsQuery(string Term, int Offset = 0, int Limit = 10) : IRequest<PagedResult<ContactDto>>;

public class SearchContactsQueryHandler : IRequestHandler<SearchContactsQuery, PagedResult<ContactDto>>
{
    private readonly IRepository<Contact> _repository;
    private readonly IMapper _mapper;

    public SearchContactsQueryHandler(IRepository<Contact> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ContactDto>> Handle(SearchContactsQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query().Where(e => !e.IsDeleted);
        // Simple search implementation
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Offset).Take(request.Limit).ToListAsync(cancellationToken);

        return new PagedResult<ContactDto> { Items = _mapper.Map<List<ContactDto>>(items), TotalCount = totalCount, Offset = request.Offset, Limit = request.Limit };
    }
}
