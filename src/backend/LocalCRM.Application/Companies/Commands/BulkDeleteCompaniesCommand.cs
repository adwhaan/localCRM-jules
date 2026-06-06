using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Companies.Commands;

public record BulkDeleteCompaniesCommand(List<int> Ids) : IRequest;

public class BulkDeleteCompaniesCommandHandler : IRequestHandler<BulkDeleteCompaniesCommand>
{
    private readonly IRepository<Company> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public BulkDeleteCompaniesCommandHandler(IRepository<Company> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(BulkDeleteCompaniesCommand request, CancellationToken cancellationToken)
    {
        var entities = await _repository.Query()
            .Where(c => request.Ids.Contains(c.CompanyId) && !c.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.Username ?? "system";
            await _audit.LogAsync("companies", entity.CompanyId, "SOFT_DELETE", entity.UpdatedBy, "Bulk delete");
        }

        await _repository.BulkUpdateAsync(entities);
    }
}
