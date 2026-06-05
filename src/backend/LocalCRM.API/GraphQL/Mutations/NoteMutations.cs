using HotChocolate;
using LocalCRM.Application.Notes.Commands;
using LocalCRM.Application.DTOs;
using MediatR;
using LocalCRM.API.GraphQL.Common;

namespace LocalCRM.API.GraphQL.Mutations;

public class NoteMutations
{
    public async Task<NoteDto> CreateNote(CreateNoteCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<NoteDto> UpdateNote(UpdateNoteCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<MutationResult> DeleteNote(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new SoftDeleteNoteCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> RestoreNote(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new RestoreNoteCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> BulkDeleteNotes(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkDeleteNotesCommand(ids));
        return new MutationResult(true);
    }

    public async Task<MutationResult> BulkRestoreNotes(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkRestoreNotesCommand(ids));
        return new MutationResult(true);
    }
}
