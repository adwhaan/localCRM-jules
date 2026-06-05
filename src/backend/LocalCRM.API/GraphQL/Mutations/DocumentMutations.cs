<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.Documents.Commands;
using LocalCRM.Application.DTOs;
using MediatR;
using LocalCRM.API.GraphQL.Common;

namespace LocalCRM.API.GraphQL.Mutations;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class DocumentMutations
{
    public async Task<DocumentDto> CreateDocument(CreateDocumentCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<DocumentDto> UpdateDocument(UpdateDocumentCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<MutationResult> DeleteDocument(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new SoftDeleteDocumentCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> RestoreDocument(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new RestoreDocumentCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> BulkDeleteDocuments(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkDeleteDocumentsCommand(ids));
        return new MutationResult(true);
    }

    public async Task<MutationResult> BulkRestoreDocuments(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkRestoreDocumentsCommand(ids));
        return new MutationResult(true);
    }
}
