using LocalCRM.Domain.Entities;
using LocalCRM.Infrastructure.Persistence;

namespace LocalCRM.API.GraphQL.Queries;

public class CompanyQueries
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Company> GetCompanies(ApplicationDbContext context)
    {
        return context.Companies.Where(c => !c.IsDeleted);
    }
}
