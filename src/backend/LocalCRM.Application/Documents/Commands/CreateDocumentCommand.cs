using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Documents.Commands;

public record CreateDocumentCommand : IRequest<DocumentDto>
{
    public string DocumentRef { get; init; } = string.Empty;
    public string? Subject { get; init; }
    public string DocumentType { get; init; } = string.Empty;
    public string DocumentUrl { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
}

public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, DocumentDto>
{
    private readonly IRepository<Document> _repository;
    private readonly IMapper _mapper;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public CreateDocumentCommandHandler(IRepository<Document> repository, IMapper mapper, IAuditService audit, ICurrentUserService currentUser)
    {
        _repository = repository;
        _mapper = mapper;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<DocumentDto> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = new Document
        {
            DocumentRef = request.DocumentRef,
            Subject = request.Subject,
            DocumentType = request.DocumentType,
            DocumentUrl = request.DocumentUrl,
            Visibility = request.Visibility,
            CreatedBy = _currentUser.Username ?? "system"
        };

        await _repository.AddAsync(entity);
        await _audit.LogAsync("documents", entity.DocumentId, "CREATE", entity.CreatedBy);

        return _mapper.Map<DocumentDto>(entity);
    }
}
