using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Companies.Queries;

public record GetPagedCompaniesQuery(int Offset = 0, int Limit = 10, bool IncludeDeleted = false) : IRequest<PagedResult<CompanyDto>>;

public class GetPagedCompaniesQueryHandler : IRequestHandler<GetPagedCompaniesQuery, PagedResult<CompanyDto>>
{
    private readonly IRepository<Company> _repository;
    private readonly IMapper _mapper;

    public GetPagedCompaniesQueryHandler(IRepository<Company> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResult<CompanyDto>> Handle(GetPagedCompaniesQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.Query();

        if (!request.IncludeDeleted)
        {
            query = query.Where(c => !c.IsDeleted);
        }
        else
        {
            query = query.Where(c => c.IsDeleted);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.Name)
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return new PagedResult<CompanyDto>
        {
            Items = _mapper.Map<List<CompanyDto>>(items),
            TotalCount = totalCount,
            Offset = request.Offset,
            Limit = request.Limit
        };
    }
}
