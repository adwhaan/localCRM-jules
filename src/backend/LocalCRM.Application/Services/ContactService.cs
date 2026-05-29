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
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact> _repo;
        private readonly IMapper _mapper;
        private readonly IAuditService _audit;

        public ContactService(IRepository<Contact> repo, IMapper mapper, IAuditService audit)
        {
            _repo = repo;
            _mapper = mapper;
            _audit = audit;
        }

        public async Task<IEnumerable<ContactDto>> GetAllAsync() => _mapper.Map<IEnumerable<ContactDto>>(await _repo.GetAllAsync());

        public async Task<IEnumerable<ContactDto>> GetDeletedAsync()
        {
            var items = await _repo.QueryIgnoreFilters().Where(c => c.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<ContactDto>>(items);
        }

        public async Task<ContactDto?> GetByIdAsync(int id) => _mapper.Map<ContactDto>(await _repo.GetByIdAsync(id));

        public async Task<ContactDto> CreateAsync(CreateContactDto dto, string username)
        {
            var item = _mapper.Map<Contact>(dto);
            item.CreatedBy = username;
            item.CreatedAt = DateTime.UtcNow;
            _repo.Add(item);
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("contacts", item.ContactId, ActionTypes.Create, null, username);
            return _mapper.Map<ContactDto>(item);
        }

        public async Task<bool> UpdateAsync(int id, UpdateContactDto dto, string username, DateTime updatedAt)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null || (item.UpdatedAt != null && item.UpdatedAt != updatedAt)) return false;
            _mapper.Map(dto, item);
            item.UpdatedBy = username;
            item.UpdatedAt = DateTime.UtcNow;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("contacts", id, ActionTypes.Update, null, username);
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
            await _audit.LogAsync("contacts", id, ActionTypes.SoftDelete, null, username);
            return true;
        }

        public async Task<bool> RestoreAsync(int id, string username)
        {
            var item = await _repo.QueryIgnoreFilters().FirstOrDefaultAsync(c => c.ContactId == id);
            if (item == null || !item.IsDeleted) return false;
            item.IsDeleted = false;
            item.DeletedAt = null;
            item.UpdatedBy = username;
            await _repo.SaveChangesAsync();
            await _audit.LogAsync("contacts", id, ActionTypes.Restore, null, username);
            return true;
        }

        public async Task<int> BulkDeleteAsync(IEnumerable<int> ids, string username)
        {
            int count = 0;
            foreach (var id in ids)
            {
                if (await DeleteAsync(id, username)) count++;
            }
            return count;
        }

        public async Task<int> BulkRestoreAsync(IEnumerable<int> ids, string username)
        {
            int count = 0;
            foreach (var id in ids)
            {
                if (await RestoreAsync(id, username)) count++;
            }
            return count;
        }

        public async Task<IEnumerable<ContactDto>> SearchAsync(string query)
        {
            var results = await _repo.Query()
                .Where(c => c.FirstName.Contains(query) || c.LastName.Contains(query) || c.Email.Contains(query))
                .ToListAsync();
            return _mapper.Map<IEnumerable<ContactDto>>(results);
        }
    }
}
