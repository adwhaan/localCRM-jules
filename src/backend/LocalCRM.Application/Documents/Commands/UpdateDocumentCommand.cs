using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Common.Exceptions;

namespace LocalCRM.Application.Documents.Commands;

public record UpdateDocumentCommand : IRequest<DocumentDto>
{
    public int DocumentId { get; init; }
    public string Subject { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
}

public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, DocumentDto>
{
    private readonly IRepository<Document> _repository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _audit;

    public UpdateDocumentCommandHandler(IRepository<Document> repository, IMapper mapper, ICurrentUserService currentUser, IAuditService audit)
    {
        _repository = repository;
        _mapper = mapper;
        _currentUser = currentUser;
        _audit = audit;
    }

    public async Task<DocumentDto> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.DocumentId);
        if (entity == null || entity.IsDeleted) throw new Exception("Not found");

        if (entity.UpdatedAt.HasValue && entity.UpdatedAt.Value != request.UpdatedAt)
            throw new ConcurrencyException("Concurrency conflict");

        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Documents", entity.DocumentId, "UPDATE", entity.UpdatedBy);
        return _mapper.Map<DocumentDto>(entity);
    }
}
