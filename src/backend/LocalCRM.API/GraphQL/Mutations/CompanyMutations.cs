<<<<<<< HEAD
=======
using HotChocolate.Authorization;
>>>>>>> feature-backend-12855298858282564638
using HotChocolate;
using LocalCRM.Application.Companies.Commands;
using LocalCRM.Application.DTOs;
using MediatR;
using LocalCRM.API.GraphQL.Common;

namespace LocalCRM.API.GraphQL.Mutations;

<<<<<<< HEAD
=======
[Authorize]
>>>>>>> feature-backend-12855298858282564638
public class CompanyMutations
{
    public async Task<CompanyDto> CreateCompany(CreateCompanyCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<CompanyDto> UpdateCompany(UpdateCompanyCommand command, [Service] IMediator mediator)
    {
        return await mediator.Send(command);
    }

    public async Task<MutationResult> DeleteCompany(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new SoftDeleteCompanyCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> RestoreCompany(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new RestoreCompanyCommand(id));
        return new MutationResult(true, id);
    }

    public async Task<MutationResult> BulkDeleteCompanies(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkDeleteCompaniesCommand(ids));
        return new MutationResult(true);
    }

    public async Task<MutationResult> BulkRestoreCompanies(List<int> ids, [Service] IMediator mediator)
    {
        await mediator.Send(new BulkRestoreCompaniesCommand(ids));
        return new MutationResult(true);
    }
}
