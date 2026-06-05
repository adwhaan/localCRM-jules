using HotChocolate;
using LocalCRM.Application.Companies.Queries;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Queries;

public class CompanyQueries
{
    [UseFiltering]
    [UseSorting]
    public async Task<PagedResult<CompanyDto>> GetCompanies(
        int offset = 0,
        int limit = 10,
        [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetPagedCompaniesQuery(offset, limit));
    }

    public async Task<CompanyDto?> GetCompany(int? id, string? @ref, [Service] IMediator mediator = null!)
    {
        if (id.HasValue && !string.IsNullOrEmpty(@ref)) throw new Exception("Provide either id or ref, not both");
        if (id.HasValue) return await mediator.Send(new GetCompanyByIdQuery(id.Value));
        if (!string.IsNullOrEmpty(@ref)) return await mediator.Send(new GetCompanyByRefQuery(@ref));
        return null;
    }

    [UseFiltering]
    [UseSorting]
    public async Task<PagedResult<CompanyDto>> GetDeletedCompanies(
        int offset = 0,
        int limit = 10,
        [Service] IMediator mediator = null!)
    {
        return await mediator.Send(new GetPagedCompaniesQuery(offset, limit, true));
    }
}
