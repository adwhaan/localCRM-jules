using HotChocolate;
using LocalCRM.Application.Companies.Commands;
using LocalCRM.Application.DTOs;
using MediatR;

namespace LocalCRM.API.GraphQL.Mutations;

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

    public async Task<bool> DeleteCompany(int id, [Service] IMediator mediator)
    {
        await mediator.Send(new SoftDeleteCompanyCommand(id));
        return true;
    }

    public async Task<CompanyDto> RestoreCompany(int id, [Service] IMediator mediator)
    {
        return await mediator.Send(new RestoreCompanyCommand(id));
    }
}
