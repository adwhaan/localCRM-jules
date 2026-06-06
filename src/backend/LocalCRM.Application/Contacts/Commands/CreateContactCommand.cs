using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using FluentValidation;

namespace LocalCRM.Application.Contacts.Commands;

public record CreateContactCommand : IRequest<ContactDto>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public int Rating { get; init; }
}

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Email).EmailAddress().When(v => !string.IsNullOrEmpty(v.Email));
        RuleFor(v => v.Rating).InclusiveBetween(0, 4);
    }
}

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactDto>
{
    private readonly IRepository<Contact> _repository;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public CreateContactCommandHandler(IRepository<Contact> repository, IMapper mapper, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<ContactDto> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        var entity = new Contact
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Rating = request.Rating,
            CreatedBy = _currentUser.Username ?? "system"
        };

        await _repository.AddAsync(entity);
        await _audit.LogAsync("contacts", entity.ContactId, "CREATE", entity.CreatedBy);

        return _mapper.Map<ContactDto>(entity);
    }
}
