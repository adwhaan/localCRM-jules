using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Common.Exceptions;

namespace LocalCRM.Application.Engagements.Commands;

public record UpdateEngagementCommand : IRequest<EngagementDto>
{
    public int EngagementId { get; init; }
    public string Subject { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
}

public class UpdateEngagementCommandHandler : IRequestHandler<UpdateEngagementCommand, EngagementDto>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _audit;

    public UpdateEngagementCommandHandler(IRepository<Engagement> repository, IMapper mapper, ICurrentUserService currentUser, IAuditService audit)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
        _audit = audit;
    }

    public async Task<EngagementDto> Handle(UpdateEngagementCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.EngagementId);
        if (entity == null || entity.IsDeleted) throw new Exception("Not found");

        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != request.UpdatedAt)
            throw new ConcurrencyException("Concurrency conflict");

        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Engagements", entity.EngagementId, "UPDATE", entity.UpdatedBy);
        return _mapper.Map<EngagementDto>(entity);
    }
}
