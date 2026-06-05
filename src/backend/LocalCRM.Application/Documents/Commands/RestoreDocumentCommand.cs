using MediatR;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;
using AutoMapper;

namespace LocalCRM.Application.Documents.Commands;

public record RestoreDocumentCommand(int Id) : IRequest<DocumentDto>;

public class RestoreDocumentCommandHandler : IRequestHandler<RestoreDocumentCommand, DocumentDto>
{
    private readonly IRepository<Document> _repository;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public RestoreDocumentCommandHandler(IRepository<Document> repository, IAuditService audit, ICurrentUserService currentUser, IMapper mapper)
    {
        _repository = repository;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<DocumentDto> Handle(RestoreDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null || !entity.IsDeleted) throw new Exception("Not found or not deleted");
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedBy = _currentUser.Username ?? "system";
        await _repository.UpdateAsync(entity);
        await _audit.LogAsync("Documents", entity.DocumentId, "RESTORE", entity.UpdatedBy);
        return _mapper.Map<DocumentDto>(entity);
    }
}
