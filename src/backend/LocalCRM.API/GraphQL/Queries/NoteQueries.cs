<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.Notes.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class NoteQueries
{
    [UseFiltering]
    [UseSorting]
    public async Task<PagedResult<NoteDto>> GetNotes(
        int offset = 0,
        int limit = 10,
        [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetPagedNotesQuery(offset, limit));
    }

    public async Task<NoteDto?> GetNote(int id, [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetNoteByIdQuery(id));
    }
}
