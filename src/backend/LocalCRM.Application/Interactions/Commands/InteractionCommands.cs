using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Interactions.Commands;

public record CreateInteractionCommand : IRequest<InteractionDto>
{
    public DateOnly InteractionDate { get; init; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly? InteractionTime { get; init; }
    public required string InteractionType { get; init; }
    public required string Subject { get; init; }
    public string? Note { get; init; }
    public required string State { get; init; }
    public bool IsTask { get; init; }

    // Linkage
    public int? ContactId { get; init; }
    public int? CompanyId { get; init; }
    public int? EngagementId { get; init; }
}

public class CreateInteractionCommandHandler : IRequestHandler<CreateInteractionCommand, InteractionDto>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IRepository<InteractionLink> _linkRepository;
    private readonly IMapper _mapper;

    public CreateInteractionCommandHandler(
<<<<<<< HEAD
        IRepository<Interaction> repository,
=======
        IRepository<Interaction> repository,
>>>>>>> origin/main
        IRepository<InteractionLink> linkRepository,
        IMapper mapper)
    {
        _repository = repository;
        _linkRepository = linkRepository;
        _mapper = mapper;
    }

    public async Task<InteractionDto> Handle(CreateInteractionCommand request, CancellationToken cancellationToken)
    {
        var interaction = new Interaction
        {
            InteractionDate = request.InteractionDate,
            InteractionTime = request.InteractionTime,
            InteractionType = request.InteractionType,
            Subject = request.Subject,
            Note = request.Note,
            State = request.State,
            IsTask = request.IsTask,
            CreatedBy = "system"
        };

        await _repository.AddAsync(interaction, cancellationToken);

        var link = new InteractionLink
        {
            InteractionId = interaction.InteractionId,
            ContactId = request.ContactId,
            CompanyId = request.ContactId != null ? null : request.CompanyId, // Mutual exclusivity
            EngagementId = request.EngagementId,
            CreatedBy = "system"
        };

        await _linkRepository.AddAsync(link, cancellationToken);

        return _mapper.Map<InteractionDto>(interaction);
    }
}

public record SoftDeleteInteractionCommand(int InteractionId) : IRequest<bool>;

public class SoftDeleteInteractionCommandHandler : IRequestHandler<SoftDeleteInteractionCommand, bool>
{
    private readonly IRepository<Interaction> _repository;

    public SoftDeleteInteractionCommandHandler(IRepository<Interaction> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(SoftDeleteInteractionCommand request, CancellationToken cancellationToken)
    {
        var interaction = await _repository.GetByIdAsync(request.InteractionId, cancellationToken);
        if (interaction == null) return false;

        interaction.IsDeleted = true;
        interaction.DeletedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(interaction, cancellationToken);
        return true;
    }
}
