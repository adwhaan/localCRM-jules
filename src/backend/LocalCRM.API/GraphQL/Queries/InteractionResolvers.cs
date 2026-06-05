<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LocalCRM.API.GraphQL.Queries;

[ExtendObjectType(typeof(InteractionDto))]
public class InteractionResolvers
{
    public async Task<ContactDto?> GetContact(
        [Parent] InteractionDto interaction,
        [Service] IRepository<InteractionLink> repository,
        [Service] IRepository<Contact> contactRepository,
        [Service] IMapper mapper)
    {
        var link = await repository.Query()
            .FirstOrDefaultAsync(l => l.InteractionId == interaction.InteractionId && !l.IsDeleted);
        if (link?.ContactId == null) return null;
        var contact = await contactRepository.GetByIdAsync(link.ContactId.Value);
        return mapper.Map<ContactDto>(contact);
    }

    public async Task<CompanyDto?> GetCompany(
        [Parent] InteractionDto interaction,
        [Service] IRepository<InteractionLink> repository,
        [Service] IRepository<Company> companyRepository,
        [Service] IMapper mapper)
    {
        var link = await repository.Query()
            .FirstOrDefaultAsync(l => l.InteractionId == interaction.InteractionId && !l.IsDeleted);
        if (link?.CompanyId == null) return null;
        var company = await companyRepository.GetByIdAsync(link.CompanyId.Value);
        return mapper.Map<CompanyDto>(company);
    }

    public async Task<EngagementDto?> GetEngagement(
        [Parent] InteractionDto interaction,
        [Service] IRepository<InteractionLink> repository,
        [Service] IRepository<Engagement> engagementRepository,
        [Service] IMapper mapper)
    {
        var link = await repository.Query()
            .FirstOrDefaultAsync(l => l.InteractionId == interaction.InteractionId && !l.IsDeleted);
        if (link?.EngagementId == null) return null;
        var engagement = await engagementRepository.GetByIdAsync(link.EngagementId.Value);
        return mapper.Map<EngagementDto>(engagement);
    }

    public async Task<List<NoteDto>> GetNotes(
        [Parent] InteractionDto interaction,
        [Service] IRepository<InteractionNoteLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Note)
            .Where(l => l.InteractionId == interaction.InteractionId && !l.IsDeleted && !l.Note.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<NoteDto>>(links.Select(l => l.Note));
    }

    public async Task<List<DocumentDto>> GetDocuments(
        [Parent] InteractionDto interaction,
        [Service] IRepository<InteractionDocumentLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Document)
            .Where(l => l.InteractionId == interaction.InteractionId && !l.IsDeleted && !l.Document.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<DocumentDto>>(links.Select(l => l.Document));
    }
}
