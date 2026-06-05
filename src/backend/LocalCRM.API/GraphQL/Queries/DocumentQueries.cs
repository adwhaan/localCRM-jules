using HotChocolate.Authorization;
using HotChocolate;
using LocalCRM.Application.Documents.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

[Authorize]
public class DocumentQueries
{
    [UseFiltering]
    [UseSorting]
    public async Task<PagedResult<DocumentDto>> GetDocuments(
        int offset = 0,
        int limit = 10,
        [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetPagedDocumentsQuery(offset, limit));
    }

    public async Task<DocumentDto?> GetDocument(int id, [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetDocumentByIdQuery(id));
    }
}
