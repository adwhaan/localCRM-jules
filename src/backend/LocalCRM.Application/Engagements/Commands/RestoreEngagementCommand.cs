using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using AutoMapper;

namespace LocalCRM.Application.Engagements.Commands;

public record RestoreEngagementCommand(int Id) : IRequest<EngagementDto>;

public class RestoreEngagementCommandHandler : IRequestHandler<RestoreEngagementCommand, EngagementDto>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public RestoreEngagementCommandHandler(IRepository<Engagement> repository, IAuditService audit, ICurrentUserService currentUser, IMapper mapper)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<EngagementDto> Handle(RestoreEngagementCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || !entity.IsDeleted) throw new Exception("Not found or not deleted");
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Engagements", entity.EngagementId, "RESTORE", entity.UpdatedBy);
        return _mapper.Map<EngagementDto>(entity);
    }
}
