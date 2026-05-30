using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Enums;
using LocalCRM.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<EngagementDto> CreateAsync(CreateEngagementDto dto, string username)
        {
            var item = _mapper.Map<Engagement>(dto);
            item.CreatedBy = username;
            item.CreatedAt = DateTime.UtcNow;
            _repo.Add(item);
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("engagements", item.EngagementId, ActionTypes.Create, null, username);
            return _mapper.Map<EngagementDto>(item);
        }

        public async Task<bool> UpdateAsync(int id, UpdateEngagementDto dto, string username, DateTime updatedAt)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null || (item.UpdatedAt != null && item.UpdatedAt != updatedAt)) return false;
            _mapper.Map(dto, item);
            item.UpdatedBy = username;
            item.UpdatedAt = DateTime.UtcNow;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("engagements", id, ActionTypes.Update, null, username);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string username)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return false;
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.UpdatedBy = username;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("engagements", id, ActionTypes.SoftDelete, null, username);
            return true;
        }

        public async Task<bool> RestoreAsync(int id, string username)
        {
            var item = await _repo.QueryIgnoreFilters().FirstOrDefaultAsync(e => e.EngagementId == id);
            if (item == null || !item.IsDeleted) return false;
            item.IsDeleted = false;
            item.DeletedAt = null;
            item.UpdatedBy = username;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("engagements", id, ActionTypes.Restore, null, username);
            return true;
        }

        public async Task<IEnumerable<EngagementDto>> SearchAsync(string query)
        {
            var items = await _repo.Query().Where(e => e.Description != null && e.Description.Contains(query)).ToListAsync();
            return _mapper.Map<IEnumerable<EngagementDto>>(items);
        }
    }

    public class NoteService : INoteService
    {
        private readonly IRepository<Note> _repo;
        private readonly IMapper _mapper;
        private readonly IAuditService _audit;

        public NoteService(IRepository<Note> repo, IMapper mapper, IAuditService audit)
        {
            _repo = repo;
            _mapper = mapper;
            _audit = audit;
        }

        public async Task<IEnumerable<NoteDto>> GetAllAsync(string username, bool isElevated)
        {
            var query = _repo.Query();
            if (!isElevated) query = query.Where(n => n.Visibility != "priv" || n.CreatedBy == username);
            return _mapper.Map<IEnumerable<NoteDto>>(await query.ToListAsync());
        }

        public async Task<NoteDto?> GetByIdAsync(int id, string username, bool isElevated)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return null;
            if (item.Visibility == "priv" && item.CreatedBy != username && !isElevated) return null;
            return _mapper.Map<NoteDto>(item);
        }

        public async Task<NoteDto> CreateAsync(CreateNoteDto dto, string username)
        {
            var item = _mapper.Map<Note>(dto);
            item.CreatedBy = username;
            item.CreatedAt = DateTime.UtcNow;
            _repo.Add(item);
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("notes", item.NoteId, ActionTypes.Create, null, username);
            return _mapper.Map<NoteDto>(item);
        }

        public async Task<bool> UpdateAsync(int id, UpdateNoteDto dto, string username, DateTime updatedAt)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null || (item.UpdatedAt != null && item.UpdatedAt != updatedAt)) return false;
            _mapper.Map(dto, item);
            item.UpdatedBy = username;
            item.UpdatedAt = DateTime.UtcNow;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("notes", id, ActionTypes.Update, null, username);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string username)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return false;
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.UpdatedBy = username;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("notes", id, ActionTypes.SoftDelete, null, username);
            return true;
        }

        public async Task<bool> RestoreAsync(int id, string username)
        {
            var item = await _repo.QueryIgnoreFilters().FirstOrDefaultAsync(n => n.NoteId == id);
            if (item == null || !item.IsDeleted) return false;
            item.IsDeleted = false;
            item.DeletedAt = null;
            item.UpdatedBy = username;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("notes", id, ActionTypes.Restore, null, username);
            return true;
        }

        public async Task<IEnumerable<NoteDto>> SearchAsync(string query, string username, bool isElevated)
        {
            var q = _repo.Query().Where(n => n.Subject.Contains(query) || (n.Body != null && n.Body.Contains(query)));
            if (!isElevated) q = q.Where(n => n.Visibility != "priv" || n.CreatedBy == username);
            return _mapper.Map<IEnumerable<NoteDto>>(await q.ToListAsync());
        }
    }

    public class DocumentService : IDocumentService
    {
        private readonly IRepository<Document> _repo;
        private readonly IMapper _mapper;
        private readonly IAuditService _audit;

        public DocumentService(IRepository<Document> repo, IMapper mapper, IAuditService audit)
        {
            _repo = repo;
            _mapper = mapper;
            _audit = audit;
        }

        public async Task<IEnumerable<DocumentDto>> GetAllAsync(string username, bool isElevated)
        {
            var query = _repo.Query();
            if (!isElevated) query = query.Where(d => d.Visibility != "priv" || d.CreatedBy == username);
            return _mapper.Map<IEnumerable<DocumentDto>>(await query.ToListAsync());
        }

        public async Task<DocumentDto?> GetByIdAsync(int id, string username, bool isElevated)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return null;
            if (item.Visibility == "priv" && item.CreatedBy != username && !isElevated) return null;
            return _mapper.Map<DocumentDto>(item);
        }

        public async Task<DocumentDto> CreateAsync(CreateDocumentDto dto, string username)
        {
            var item = _mapper.Map<Document>(dto);
            item.CreatedBy = username;
            item.CreatedAt = DateTime.UtcNow;
            _repo.Add(item);
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("documents", item.DocumentId, ActionTypes.Create, null, username);
            return _mapper.Map<DocumentDto>(item);
        }

        public async Task<bool> UpdateAsync(int id, UpdateDocumentDto dto, string username, DateTime updatedAt)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null || (item.UpdatedAt != null && item.UpdatedAt != updatedAt)) return false;
            _mapper.Map(dto, item);
            item.UpdatedBy = username;
            item.UpdatedAt = DateTime.UtcNow;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("documents", id, ActionTypes.Update, null, username);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string username)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return false;
            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.UpdatedBy = username;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("documents", id, ActionTypes.SoftDelete, null, username);
            return true;
        }

        public async Task<bool> RestoreAsync(int id, string username)
        {
            var item = await _repo.QueryIgnoreFilters().FirstOrDefaultAsync(d => d.DocumentId == id);
            if (item == null || !item.IsDeleted) return false;
            item.IsDeleted = false;
            item.DeletedAt = null;
            item.UpdatedBy = username;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("documents", id, ActionTypes.Restore, null, username);
            return true;
        }

        public async Task<IEnumerable<DocumentDto>> SearchAsync(string query, string username, bool isElevated)
        {
            var q = _repo.Query().Where(d => d.Subject != null && d.Subject.Contains(query));
            if (!isElevated) q = q.Where(d => d.Visibility != "priv" || d.CreatedBy == username);
            return _mapper.Map<IEnumerable<DocumentDto>>(await q.ToListAsync());
        }
    }
}
