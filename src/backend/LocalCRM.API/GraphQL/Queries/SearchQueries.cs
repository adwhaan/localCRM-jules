<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.Search.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class SearchQueries
{
    public async Task<List<SearchResultDto>> Search(string term, [Service] IMediator mediator)
    {
        return await mediator.Send(new GlobalSearchQuery(term));
    }
}
