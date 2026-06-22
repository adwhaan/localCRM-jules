using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Engagements.Commands;

public record CreateEngagementCommand : IRequest<EngagementDto>
{
    public string? EngagementRef { get; init; }
    public string? Description { get; init; }
    public required string EngagementStatus { get; init; }
}

public class CreateEngagementCommandHandler : IRequestHandler<CreateEngagementCommand, EngagementDto>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IMapper _mapper;

    public CreateEngagementCommandHandler(IRepository<Engagement> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EngagementDto> Handle(CreateEngagementCommand request, CancellationToken cancellationToken)
    {
        var engagement = new Engagement
        {
            EngagementRef = request.EngagementRef,
            Description = request.Description,
            EngagementStatus = request.EngagementStatus,
            CreatedBy = "system"
        };

        await _repository.AddAsync(engagement, cancellationToken);
        return _mapper.Map<EngagementDto>(engagement);
    }
}

public record UpdateEngagementCommand : IRequest<EngagementDto>
{
    public int EngagementId { get; init; }
    public string? Description { get; init; }
    public string EngagementStatus { get; init; } = string.Empty;
}

public class UpdateEngagementCommandHandler : IRequestHandler<UpdateEngagementCommand, EngagementDto>
{
    private readonly IRepository<Engagement> _repository;
    private readonly IMapper _mapper;

    public UpdateEngagementCommandHandler(IRepository<Engagement> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EngagementDto> Handle(UpdateEngagementCommand request, CancellationToken cancellationToken)
    {
        var engagement = await _repository.GetByIdAsync(request.EngagementId, cancellationToken);
        if (engagement == null) throw new KeyNotFoundException("Engagement not found");

        engagement.Description = request.Description;
        engagement.EngagementStatus = request.EngagementStatus;
        engagement.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(engagement, cancellationToken);
        return _mapper.Map<EngagementDto>(engagement);
    }
}

public record SoftDeleteEngagementCommand(int EngagementId) : IRequest<bool>;

public class SoftDeleteEngagementCommandHandler : IRequestHandler<SoftDeleteEngagementCommand, bool>
{
    private readonly IRepository<Engagement> _repository;

    public SoftDeleteEngagementCommandHandler(IRepository<Engagement> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(SoftDeleteEngagementCommand request, CancellationToken cancellationToken)
    {
        var engagement = await _repository.GetByIdAsync(request.EngagementId, cancellationToken);
        if (engagement == null) return false;

        engagement.IsDeleted = true;
        engagement.DeletedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(engagement, cancellationToken);
        return true;
    }
}
