using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Common.Exceptions;

namespace LocalCRM.Application.Interactions.Commands;

public record UpdateInteractionCommand : IRequest<InteractionDto>
{
    public int InteractionId { get; init; }
    public string Subject { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
}

public class UpdateInteractionCommandHandler : IRequestHandler<UpdateInteractionCommand, InteractionDto>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _audit;

    public UpdateInteractionCommandHandler(IRepository<Interaction> repository, IMapper mapper, ICurrentUserService currentUser, IAuditService audit)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
        _audit = audit;
    }

    public async Task<InteractionDto> Handle(UpdateInteractionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.InteractionId);
        if (entity == null || entity.IsDeleted) throw new Exception("Not found");

        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != request.UpdatedAt)
            throw new ConcurrencyException("Concurrency conflict");

        entity.Subject = request.Subject;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("interactions", entity.InteractionId, "UPDATE", entity.UpdatedBy);
        return _mapper.Map<InteractionDto>(entity);
    }
}
