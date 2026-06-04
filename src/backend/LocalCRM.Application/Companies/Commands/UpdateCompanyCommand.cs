using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Companies.Commands;

public record UpdateCompanyCommand : IRequest<CompanyDto>
{
    public int CompanyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string CompanyType { get; init; } = string.Empty;
    public int Rating { get; init; }
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, CompanyDto>
{
    private readonly IRepository<Company> _repository;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public UpdateCompanyCommandHandler(IRepository<Company> repository, IMapper mapper, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<CompanyDto> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.CompanyId);
        if (entity == null || entity.IsDeleted) throw new Exception("Company not found");

        entity.Name = request.Name;
        entity.City = request.City;
        entity.CompanyType = request.CompanyType;
        entity.Rating = request.Rating;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";

        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("companies", entity.CompanyId, "UPDATE", entity.UpdatedBy);

        return _mapper.Map<CompanyDto>(entity);
    }
}
