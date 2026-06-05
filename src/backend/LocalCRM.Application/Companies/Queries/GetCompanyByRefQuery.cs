using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Companies.Queries;

public record GetCompanyByRefQuery(string Ref) : IRequest<CompanyDto?>;

public class GetCompanyByRefQueryHandler : IRequestHandler<GetCompanyByRefQuery, CompanyDto?>
{
    private readonly IRepository<Company> _repository;
    private readonly IMapper _mapper;

    public GetCompanyByRefQueryHandler(IRepository<Company> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CompanyDto?> Handle(GetCompanyByRefQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.Query()
            .FirstOrDefaultAsync(c => c.CompanyRef == request.Ref && !c.IsDeleted, cancellationToken);
        return _mapper.Map<CompanyDto>(entity);
    }
}
