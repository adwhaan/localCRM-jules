using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;

namespace LocalCRM.Application.Companies.Commands;

public record LinkContactToCompanyCommand(int CompanyId, int ContactId, DateOnly StartDate, string? Role) : IRequest;

public class LinkContactToCompanyCommandHandler : IRequestHandler<LinkContactToCompanyCommand>
{
    private readonly IRepository<CompanyContactLink> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public LinkContactToCompanyCommandHandler(IRepository<CompanyContactLink> repository, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(LinkContactToCompanyCommand request, CancellationToken cancellationToken)
    {
        var link = new CompanyContactLink
        {
            CompanyId = request.CompanyId,
            ContactId = request.ContactId,
            StartDate = request.StartDate,
            Role = request.Role,
            CreatedBy = _currentUser.Username ?? "system"
        };

        await _repository.AddAsync(link);
        await _audit.LogAsync("company_contacts_link", request.CompanyId, "LINK_ADD", link.CreatedBy, $"ContactId: {request.ContactId}");
    }
}
