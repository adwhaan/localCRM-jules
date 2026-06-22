using LocalCRM.Application.Interfaces;
using LocalCRM.Application.DTOs;
using LocalCRM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Tag = LocalCRM.Domain.Entities.Tag;

namespace LocalCRM.API.Controllers;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/[controller]")]
public class AuditController : ApiControllerBase
{
    private readonly IRepository<AuditLog> _repository;

    public AuditController(IRepository<AuditLog> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<AuditLog>>> Get(int offset = 0, int limit = 10)
    {
        var query = _repository.Query().OrderByDescending(a => a.PerformedAt);
        var totalCount = await query.CountAsync();
        var items = await query.Skip(offset).Take(limit).ToListAsync();

        return new PagedResult<AuditLog>
        {
            Items = items,
            TotalCount = totalCount,
            Offset = offset,
            Limit = limit
        };
    }
}

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/[controller]")]
public class TagsController : ApiControllerBase
{
    private readonly IRepository<Tag> _repository;

    public TagsController(IRepository<Tag> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<Tag>>> Get(int offset = 0, int limit = 10)
    {
        var query = _repository.Query().OrderBy(t => t.TagGroup).ThenBy(t => t.TagOrder);
        var totalCount = await query.CountAsync();
        var items = await query.Skip(offset).Take(limit).ToListAsync();

        return new PagedResult<Tag>
        {
            Items = items,
            TotalCount = totalCount,
            Offset = offset,
            Limit = limit
        };
    }
}
