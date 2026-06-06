using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Common.Exceptions;

namespace LocalCRM.Application.Contacts.Commands;

public record UpdateContactCommand : IRequest<ContactDto>
{
    public int ContactId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public int Rating { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, ContactDto>
{
    private readonly IRepository<Contact> _repository;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public UpdateContactCommandHandler(IRepository<Contact> repository, IMapper mapper, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<ContactDto> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.ContactId);
        if (entity == null || entity.IsDeleted) throw new Exception("Contact not found");

        if (request.UpdatedAt.HasValue && entity.UpdatedAt.HasValue && request.UpdatedAt.Value != entity.UpdatedAt.Value)
        {
            throw new ConcurrencyException("Contact was modified by another user.");
        }

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.Rating = request.Rating;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";

        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("contacts", entity.ContactId, "UPDATE", entity.UpdatedBy);

        return _mapper.Map<ContactDto>(entity);
    }
}
