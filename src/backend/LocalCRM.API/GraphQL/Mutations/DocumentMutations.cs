using HotChocolate;
using LocalCRM.Application.Documents.Commands;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Mutations;

public class DocumentMutations
{
    public async Task<DocumentDto> CreateDocument(CreateDocumentCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }
}
