using MediatR;
using AutoMapper;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Application.DTOs;

namespace LocalCRM.Application.Documents.Queries;

public record GetDocumentsQuery : IRequest<List<DocumentDto>>;

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, List<DocumentDto>>
{
    private readonly IRepository<Document> _repository;
    private readonly IMapper _mapper;

    public GetDocumentsQueryHandler(IRepository<Document> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<List<DocumentDto>>(entities.Where(e => !e.IsDeleted).ToList());
    }
}
