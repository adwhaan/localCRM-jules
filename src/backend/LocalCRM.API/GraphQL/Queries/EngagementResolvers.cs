using HotChocolate;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LocalCRM.API.GraphQL.Queries;

[ExtendObjectType(typeof(EngagementDto))]
public class EngagementResolvers
{
    public async Task<List<CompanyDto>> GetCompanies(
        [Parent] EngagementDto engagement,
        [Service] IRepository<EngagementCompanyLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Company)
            .Where(l => l.EngagementId == engagement.EngagementId && !l.IsDeleted && !l.Company.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<CompanyDto>>(links.Select(l => l.Company));
    }

    public async Task<List<ContactDto>> GetContacts(
        [Parent] EngagementDto engagement,
        [Service] IRepository<EngagementContactLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Contact)
            .Where(l => l.EngagementId == engagement.EngagementId && !l.IsDeleted && !l.Contact.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<ContactDto>>(links.Select(l => l.Contact));
    }

    public async Task<List<NoteDto>> GetNotes(
        [Parent] EngagementDto engagement,
        [Service] IRepository<EngagementNoteLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Note)
            .Where(l => l.EngagementId == engagement.EngagementId && !l.IsDeleted && !l.Note.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<NoteDto>>(links.Select(l => l.Note));
    }

    public async Task<List<DocumentDto>> GetDocuments(
        [Parent] EngagementDto engagement,
        [Service] IRepository<EngagementDocumentLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Document)
            .Where(l => l.EngagementId == engagement.EngagementId && !l.IsDeleted && !l.Document.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<DocumentDto>>(links.Select(l => l.Document));
    }
}
