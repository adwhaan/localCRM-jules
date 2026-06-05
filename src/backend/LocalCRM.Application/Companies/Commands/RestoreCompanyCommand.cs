using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using AutoMapper;

namespace LocalCRM.Application.Companies.Commands;

public record RestoreCompanyCommand(int Id) : IRequest<CompanyDto>;

public class RestoreCompanyCommandHandler : IRequestHandler<RestoreCompanyCommand, CompanyDto>
{
    private readonly IRepository<Company> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public RestoreCompanyCommandHandler(IRepository<Company> repository, IAuditService audit, ICurrentUserService currentUser, IMapper mapper)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<CompanyDto> Handle(RestoreCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || !entity.IsDeleted) throw new Exception("Company not found or not deleted");

        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedBy = _currentUser.Username ?? "system";

        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("companies", entity.CompanyId, "RESTORE", entity.UpdatedBy);

        return _mapper.Map<CompanyDto>(entity);
    }
}
