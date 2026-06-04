using HotChocolate;
using LocalCRM.Application.Documents.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

public class DocumentQueries
{
    public async Task<List<DocumentDto>> GetDocuments([Service] IMediator mediator)
    {
        return await mediator.Send(new GetDocumentsQuery());
    }
}
