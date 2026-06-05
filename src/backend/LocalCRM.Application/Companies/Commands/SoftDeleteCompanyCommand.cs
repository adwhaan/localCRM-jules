using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Companies.Commands;

public record SoftDeleteCompanyCommand(int Id) : IRequest;

public class SoftDeleteCompanyCommandHandler : IRequestHandler<SoftDeleteCompanyCommand>
{
    private readonly IRepository<Company> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public SoftDeleteCompanyCommandHandler(IRepository<Company> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(SoftDeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || entity.IsDeleted) return;

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";

        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("companies", entity.CompanyId, "SOFT_DELETE", entity.UpdatedBy);
    }
}
