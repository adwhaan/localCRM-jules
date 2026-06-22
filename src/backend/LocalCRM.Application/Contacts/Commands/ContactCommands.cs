using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Contacts.Commands;

public record CreateContactCommand : IRequest<ContactDto>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? Email { get; init; }
    public int Rating { get; init; }
}

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactDto>
{
    private readonly IRepository<Contact> _repository;
    private readonly IMapper _mapper;

    public CreateContactCommandHandler(IRepository<Contact> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ContactDto> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = new Contact
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Rating = request.Rating,
            CreatedBy = "system"
        };

        await _repository.AddAsync(contact, cancellationToken);
        return _mapper.Map<ContactDto>(contact);
    }
}

public record UpdateContactCommand : IRequest<ContactDto>
{
    public int ContactId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public int Rating { get; init; }
}

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, ContactDto>
{
    private readonly IRepository<Contact> _repository;
    private readonly IMapper _mapper;

    public UpdateContactCommandHandler(IRepository<Contact> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ContactDto> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _repository.GetByIdAsync(request.ContactId, cancellationToken);
        if (contact == null) throw new KeyNotFoundException("Contact not found");

        contact.FirstName = request.FirstName;
        contact.LastName = request.LastName;
        contact.Rating = request.Rating;
        contact.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(contact, cancellationToken);
        return _mapper.Map<ContactDto>(contact);
    }
}

public record SoftDeleteContactCommand(int ContactId) : IRequest<bool>;

public class SoftDeleteContactCommandHandler : IRequestHandler<SoftDeleteContactCommand, bool>
{
    private readonly IRepository<Contact> _repository;

    public SoftDeleteContactCommandHandler(IRepository<Contact> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(SoftDeleteContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _repository.GetByIdAsync(request.ContactId, cancellationToken);
        if (contact == null) return false;

        contact.IsDeleted = true;
        contact.DeletedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(contact, cancellationToken);
        return true;
    }
}
