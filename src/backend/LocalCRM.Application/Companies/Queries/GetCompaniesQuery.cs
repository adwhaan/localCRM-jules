using MediatR;
using Microsoft.EntityFrameworkCore;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LocalCRM.Application.Companies.Queries;

public record GetCompaniesQuery : IRequest<List<Company>>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, List<Company>>
{
    // Normally would use a DB context or repository here
    // For now, this is a placeholder to show structure
    public Task<List<Company>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new List<Company>());
    }
}
