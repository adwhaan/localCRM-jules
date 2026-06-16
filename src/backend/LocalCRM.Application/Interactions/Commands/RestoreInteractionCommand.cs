using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using AutoMapper;

namespace LocalCRM.Application.Interactions.Commands;

public record RestoreInteractionCommand(int Id) : IRequest<InteractionDto>;

public class RestoreInteractionCommandHandler : IRequestHandler<RestoreInteractionCommand, InteractionDto>
{
    private readonly IRepository<Interaction> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public RestoreInteractionCommandHandler(IRepository<Interaction> repository, IAuditService audit, ICurrentUserService currentUser, IMapper mapper)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<InteractionDto> Handle(RestoreInteractionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || !entity.IsDeleted) throw new Exception("Not found or not deleted");
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Interactions", entity.InteractionId, "RESTORE", entity.UpdatedBy);
        return _mapper.Map<InteractionDto>(entity);
    }
}
