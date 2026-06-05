using HotChocolate;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LocalCRM.API.GraphQL.Queries;

[ExtendObjectType(typeof(CompanyDto))]
public class CompanyResolvers
{
    public async Task<List<ContactDto>> GetContacts(
        [Parent] CompanyDto company,
        [Service] IRepository<CompanyContactLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Contact)
            .Where(l => l.CompanyId == company.CompanyId && !l.IsDeleted && !l.Contact.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<ContactDto>>(links.Select(l => l.Contact));
    }

    public async Task<List<NoteDto>> GetNotes(
        [Parent] CompanyDto company,
        [Service] IRepository<CompanyNoteLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Note)
            .Where(l => l.CompanyId == company.CompanyId && !l.IsDeleted && !l.Note.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<NoteDto>>(links.Select(l => l.Note));
    }

    public async Task<List<DocumentDto>> GetDocuments(
        [Parent] CompanyDto company,
        [Service] IRepository<CompanyDocumentLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Document)
            .Where(l => l.CompanyId == company.CompanyId && !l.IsDeleted && !l.Document.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<DocumentDto>>(links.Select(l => l.Document));
    }
}
