using MediatR;
using FluentValidation;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Companies.Commands;

public record CreateCompanyCommand : IRequest<CompanyDto>
{
    public string CompanyRef { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string CompanyType { get; init; } = string.Empty;
    public int Rating { get; init; }
}

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(v => v.CompanyRef).NotEmpty().MaximumLength(50);
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.City).NotEmpty();
        RuleFor(v => v.CompanyType).NotEmpty();
        RuleFor(v => v.Rating).InclusiveBetween(0, 4);
    }
}

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, CompanyDto>
{
    private readonly IRepository<Company> _repository;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;

    public CreateCompanyCommandHandler(IRepository<Company> repository, IMapper mapper, IAuditService audit)
    {
        _repository = repository;
        _mapper = mapper;
        _audit = audit;
    }

    public async Task<CompanyDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = new Company
        {
            CompanyRef = request.CompanyRef,
            Name = request.Name,
            City = request.City,
            CompanyType = request.CompanyType,
            Rating = request.Rating,
            CreatedBy = "system" // Should come from ICurrentUser interface later
        };

        await _repository.AddAsync(entity);
        await _audit.LogAsync("companies", entity.CompanyId, "CREATE", entity.CreatedBy);

        return _mapper.Map<CompanyDto>(entity);
    }
}
