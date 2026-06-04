using HotChocolate;
using LocalCRM.Application.Notes.Commands;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Mutations;

public class NoteMutations
{
    public async Task<NoteDto> CreateNote(CreateNoteCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }
}
