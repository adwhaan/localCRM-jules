using HotChocolate.Authorization;
using HotChocolate;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LocalCRM.API.GraphQL.Queries;

[ExtendObjectType(typeof(ContactDto))]
public class ContactResolvers
{
    public async Task<List<NoteDto>> GetNotes(
        [Parent] ContactDto contact,
        [Service] IRepository<ContactNoteLink> repository,
        [Service] IMapper mapper)
    {
        var links = await repository.Query()
            .Include(l => l.Note)
            .Where(l => l.ContactId == contact.ContactId && !l.IsDeleted && !l.Note.IsDeleted)
            .ToListAsync();
        return mapper.Map<List<NoteDto>>(links.Select(l => l.Note));
    }
}
