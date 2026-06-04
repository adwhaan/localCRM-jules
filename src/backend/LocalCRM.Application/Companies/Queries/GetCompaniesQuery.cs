using MediatR;
using Microsoft.EntityFrameworkCore;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Companies.Queries;

public record GetCompaniesQuery : IRequest<List<CompanyDto>>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, List<CompanyDto>>
{
    private readonly IRepository<Company> _repository;
    private readonly IMapper _mapper;

    public GetCompaniesQueryHandler(IRepository<Company> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<CompanyDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync();
        // Filter out deleted in the repository or here if not using Global Query Filters
        return _mapper.Map<List<CompanyDto>>(entities.Where(c => !c.IsDeleted).ToList());
    }
}
