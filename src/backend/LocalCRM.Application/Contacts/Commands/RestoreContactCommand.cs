using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using AutoMapper;

namespace LocalCRM.Application.Contacts.Commands;

public record RestoreContactCommand(int Id) : IRequest<ContactDto>;

public class RestoreContactCommandHandler : IRequestHandler<RestoreContactCommand, ContactDto>
{
    private readonly IRepository<Contact> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public RestoreContactCommandHandler(IRepository<Contact> repository, IAuditService audit, ICurrentUserService currentUser, IMapper mapper)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ContactDto> Handle(RestoreContactCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || !entity.IsDeleted) throw new Exception("Contact not found or not deleted");

        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedBy = _currentUser.Username ?? "system";

        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("contacts", entity.ContactId, "RESTORE", entity.UpdatedBy);

        return _mapper.Map<ContactDto>(entity);
    }
}
