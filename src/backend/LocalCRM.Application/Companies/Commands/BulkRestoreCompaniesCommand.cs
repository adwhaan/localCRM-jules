using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Companies.Commands;

public record BulkRestoreCompaniesCommand(List<int> Ids) : IRequest;

public class BulkRestoreCompaniesCommandHandler : IRequestHandler<BulkRestoreCompaniesCommand>
{
    private readonly IRepository<Company> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public BulkRestoreCompaniesCommandHandler(IRepository<Company> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(BulkRestoreCompaniesCommand request, CancellationToken cancellationToken)
    {
        var entities = await _repository.Query()
            .Where(c => request.Ids.Contains(c.CompanyId) && c.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.UpdatedBy = _currentUser.Username ?? "system";
            await _audit.LogAsync("companies", entity.CompanyId, "RESTORE", entity.UpdatedBy, "Bulk restore");
        }

        await _repository.BulkUpdateAsync(entities);
    }
}
