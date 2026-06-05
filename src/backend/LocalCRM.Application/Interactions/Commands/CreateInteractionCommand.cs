using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using FluentValidation;

namespace LocalCRM.Application.Interactions.Commands;

public record CreateInteractionCommand : IRequest<InteractionDto>
{
    public DateOnly InteractionDate { get; init; }
    public string InteractionType { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public bool IsTask { get; init; }
    public string State { get; init; } = "open";

    // Optional links
    public int? ContactId { get; init; }
    public int? CompanyId { get; init; }
    public int? EngagementId { get; init; }
}

public class CreateInteractionCommandHandler : IRequestHandler<CreateInteractionCommand, InteractionDto>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IRepository<InteractionLink> _linkRepository;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public CreateInteractionCommandHandler(
        IRepository<Interaction> repository,
        IRepository<InteractionLink> linkRepository,
        IMapper mapper,
        IAuditService audit,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _linkRepository = linkRepository;
        _mapper = mapper;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<InteractionDto> Handle(CreateInteractionCommand request, CancellationToken cancellationToken)
    {
        var entity = new Interaction
        {
            InteractionDate = request.InteractionDate,
            InteractionType = request.InteractionType,
            Subject = request.Subject,
            IsTask = request.IsTask,
            State = request.State,
            CreatedBy = _currentUser.Username ?? "system"
        };

        await _repository.AddAsync(entity);

        // Handle InteractionLink
        if (request.ContactId.HasValue || request.CompanyId.HasValue || request.EngagementId.HasValue)
        {
            var link = new InteractionLink
            {
                InteractionId = entity.InteractionId,
                ContactId = request.ContactId,
                CompanyId = request.ContactId.HasValue ? null : request.CompanyId, // Exclusivity rule
                EngagementId = request.EngagementId,
                CreatedBy = entity.CreatedBy
            };
            await _linkRepository.AddAsync(link);
            await _audit.LogAsync("interactions_link", entity.InteractionId, "LINK_ADD", entity.CreatedBy);
        }

        await _audit.LogAsync("interactions", entity.InteractionId, "CREATE", entity.CreatedBy);

        return _mapper.Map<InteractionDto>(entity);
    }
}
