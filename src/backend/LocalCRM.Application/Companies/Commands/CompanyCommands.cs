using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Companies.Commands;

public record CreateCompanyCommand : IRequest<CompanyDto>
{
    public required string CompanyRef { get; init; }
    public required string Name { get; init; }
    public string? AddressLine1 { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public required string CompanyType { get; init; }
    public int Rating { get; init; }
}

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    private readonly IRepository<Company> _repository;
    private readonly IMapper _mapper;

    public CreateCompanyCommandHandler(IRepository<Company> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = new Company
        {
            CompanyRef = request.CompanyRef,
            Name = request.Name,
            AddressLine1 = request.AddressLine1,
            City = request.City ?? "Unknown",
            Country = request.Country,
            CompanyType = request.CompanyType,
            Rating = request.Rating,
            CreatedBy = "system" // Should come from ICurrentUserService
        };

        await _repository.AddAsync(company, cancellationToken);
        return _mapper.Map<CompanyDto>(company);
    }
}

public record UpdateCompanyCommand : IRequest<CompanyDto>
{
    public int CompanyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? City { get; init; }
    public int Rating { get; init; }
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, CompanyDto>
{
    private readonly IRepository<Company> _repository;
    private readonly IMapper _mapper;

    public UpdateCompanyCommandHandler(IRepository<Company> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _repository.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company == null) throw new KeyNotFoundException("Company not found");

        company.Name = request.Name;
        company.City = request.City ?? company.City;
        company.Rating = request.Rating;
        company.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(company, cancellationToken);
        return _mapper.Map<CompanyDto>(company);
    }
}

public record SoftDeleteCompanyCommand(int CompanyId) : IRequest<bool>;

public class SoftDeleteCompanyCommandHandler : IRequestHandler<SoftDeleteCompanyCommand, bool>
{
    private readonly IRepository<Company> _repository;

    public SoftDeleteCompanyCommandHandler(IRepository<Company> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(SoftDeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _repository.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company == null) return false;

        company.IsDeleted = true;
        company.DeletedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(company, cancellationToken);
        return true;
    }
}
