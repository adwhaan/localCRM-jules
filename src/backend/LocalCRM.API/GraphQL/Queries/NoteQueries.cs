using HotChocolate;
using LocalCRM.Application.Notes.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

public class NoteQueries
{
    public async Task<List<NoteDto>> GetNotes([Service] IMediator mediator)
    {
        return await mediator.Send(new GetNotesQuery());
    }
}
