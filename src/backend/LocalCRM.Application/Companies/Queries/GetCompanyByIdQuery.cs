using MediatR;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using AutoMapper;

namespace LocalCRM.Application.Companies.Queries;

public record GetCompanyByIdQuery(int Id) : IRequest<CompanyDto?>;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, CompanyDto?>
{
    private readonly IRepository<Company> _repository;
    private readonly IMapper _mapper;

    public GetCompanyByIdQueryHandler(IRepository<Company> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CompanyDto?> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        return entity == null || entity.IsDeleted ? null : _mapper.Map<CompanyDto>(entity);
    }
}
