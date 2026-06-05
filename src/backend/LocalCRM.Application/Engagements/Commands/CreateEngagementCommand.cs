using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using FluentValidation;

namespace LocalCRM.Application.Engagements.Commands;

public record CreateEngagementCommand : IRequest<EngagementDto>
{
    public string? EngagementRef { get; init; }
    public string? Description { get; init; }
    public string EngagementStatus { get; init; } = string.Empty;
}

public class CreateEngagementCommandHandler : IRequestHandler<CreateEngagementCommand, EngagementDto>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public CreateEngagementCommandHandler(IRepository<Engagement> repository, IMapper mapper, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<EngagementDto> Handle(CreateEngagementCommand request, CancellationToken cancellationToken)
    {
        var entity = new Engagement
        {
            EngagementRef = request.EngagementRef,
            Description = request.Description,
            EngagementStatus = request.EngagementStatus,
            CreatedBy = _currentUser.Username ?? "system"
        };

        await _repository.AddAsync(entity);
        await _audit.LogAsync("engagements", entity.EngagementId, "CREATE", entity.CreatedBy);

        return _mapper.Map<EngagementDto>(entity);
    }
}
