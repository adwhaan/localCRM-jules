using MediatR;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LocalCRM.Application.Search.Queries;

public record GlobalSearchQuery(string Term) : IRequest<List<SearchResultDto>>;

public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, List<SearchResultDto>>
{
    private readonly IRepository<Company> _companyRepo;
    private readonly IRepository<Contact> _contactRepo;
    private readonly IRepository<Engagement> _engagementRepo;

    public GlobalSearchQueryHandler(
        IRepository<Company> companyRepo,
        IRepository<Contact> contactRepo,
        IRepository<Engagement> engagementRepo)
    {
        _companyRepo = companyRepo;
        _contactRepo = contactRepo;
        _engagementRepo = engagementRepo;
    }

    public async Task<List<SearchResultDto>> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        var results = new List<SearchResultDto>();
        var term = request.Term.ToLower();

        // 1. Engagements (Top Priority)
        var engagements = await _engagementRepo.Query()
            .Where(e => !e.IsDeleted && (e.Description!.ToLower().Contains(term) || e.EngagementRef!.ToLower().Contains(term)))
            .Select(e => new SearchResultDto { Id = e.EngagementId, EntityType = "Engagement", Title = e.EngagementRef ?? "Engagement", Subtitle = e.Description })
            .ToListAsync(cancellationToken);
        results.AddRange(engagements);

        // 2. Companies
        var companies = await _companyRepo.Query()
            .Where(c => !c.IsDeleted && (c.Name.ToLower().Contains(term) || c.CompanyRef.ToLower().Contains(term)))
            .Select(c => new SearchResultDto { Id = c.CompanyId, EntityType = "Company", Title = c.Name, Subtitle = c.City })
            .ToListAsync(cancellationToken);
        results.AddRange(companies);

        // 3. Contacts
        var contacts = await _contactRepo.Query()
            .Where(c => !c.IsDeleted && (c.FirstName.ToLower().Contains(term) || c.LastName.ToLower().Contains(term)))
            .Select(c => new SearchResultDto { Id = c.ContactId, EntityType = "Contact", Title = $"{c.FirstName} {c.LastName}", Subtitle = c.Email })
            .ToListAsync(cancellationToken);
        results.AddRange(contacts);

        return results;
    }
}
