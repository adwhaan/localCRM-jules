using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Enums;
using LocalCRM.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.Application.Services
{
    public class EngagementService : IEngagementService
    {
        private readonly IRepository<Engagement> _repo;
        private readonly IMapper _mapper;
        private readonly IAuditService _audit;

        public EngagementService(IRepository<Engagement> repo, IMapper mapper, IAuditService audit)
        {
            _repo = repo;
            _mapper = mapper;
            _audit = audit;
        }

        public async Task<IEnumerable<EngagementDto>> GetAllAsync() => _mapper.Map<IEnumerable<EngagementDto>>(await _repo.GetAllAsync());
        public async Task<EngagementDto?> GetByIdAsync(int id) => _mapper.Map<EngagementDto>(await _repo.GetByIdAsync(id));
        public async Task<EngagementDto> CreateAsync(CreateEngagementDto dto, string username) {
            var item = _mapper.Map<Engagement>(dto); item.CreatedBy = username; _repo.Add(item); await _repo.SaveChangesAsync();
            return _mapper.Map<EngagementDto>(item);
        }
        public async Task<bool> UpdateAsync(int id, UpdateEngagementDto dto, string username, DateTime updatedAt) { return false; }
        public async Task<bool> DeleteAsync(int id, string username) { return false; }
        public async Task<bool> RestoreAsync(int id, string username) { return false; }
    }

    public class NoteService : INoteService
    {
        private readonly IRepository<Note> _repo;
        private readonly IMapper _mapper;
        public NoteService(IRepository<Note> repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
        public async Task<IEnumerable<NoteDto>> GetAllAsync() => _mapper.Map<IEnumerable<NoteDto>>(await _repo.GetAllAsync());
        public async Task<NoteDto?> GetByIdAsync(int id) => _mapper.Map<NoteDto>(await _repo.GetByIdAsync(id));
        public async Task<NoteDto> CreateAsync(CreateNoteDto dto, string username) { return new NoteDto(); }
        public async Task<bool> UpdateAsync(int id, UpdateNoteDto dto, string username, DateTime updatedAt) => false;
        public async Task<bool> DeleteAsync(int id, string username) => false;
        public async Task<bool> RestoreAsync(int id, string username) => false;
    }

    public class DocumentService : IDocumentService
    {
        private readonly IRepository<Document> _repo;
        private readonly IMapper _mapper;
        public DocumentService(IRepository<Document> repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
        public async Task<IEnumerable<DocumentDto>> GetAllAsync() => _mapper.Map<IEnumerable<DocumentDto>>(await _repo.GetAllAsync());
        public async Task<DocumentDto?> GetByIdAsync(int id) => _mapper.Map<DocumentDto>(await _repo.GetByIdAsync(id));
        public async Task<DocumentDto> CreateAsync(CreateDocumentDto dto, string username) { return new DocumentDto(); }
        public async Task<bool> UpdateAsync(int id, UpdateDocumentDto dto, string username, DateTime updatedAt) => false;
        public async Task<bool> DeleteAsync(int id, string username) => false;
        public async Task<bool> RestoreAsync(int id, string username) => false;
    }
}
