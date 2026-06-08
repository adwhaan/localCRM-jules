using HotChocolate.Authorization;
using HotChocolate;
using LocalCRM.Application.Search.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

[Authorize]
[ExtendObjectType("Query")]
public class SearchQueries
{
    public async Task<List<SearchResultDto>> Search(string term, [Service] IMediator mediator)
    {
        return await mediator.Send(new GlobalSearchQuery(term));
    }
}
