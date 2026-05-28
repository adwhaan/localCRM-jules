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
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact> _repo;
        private readonly IMapper _mapper;

        public ContactService(IRepository<Contact> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContactDto>> GetAllAsync() => _mapper.Map<IEnumerable<ContactDto>>(await _repo.GetAllAsync());
        public async Task<ContactDto?> GetByIdAsync(int id) => _mapper.Map<ContactDto>(await _repo.GetByIdAsync(id));
        public async Task<ContactDto> CreateAsync(CreateContactDto dto, string username) { return new ContactDto(); }
        public async Task<bool> UpdateAsync(int id, UpdateContactDto dto, string username, DateTime updatedAt) => false;
        public async Task<bool> DeleteAsync(int id, string username) => false;
        public async Task<bool> RestoreAsync(int id, string username) => false;
        public Task<int> BulkDeleteAsync(IEnumerable<int> ids, string username) => Task.FromResult(0);
        public Task<int> BulkRestoreAsync(IEnumerable<int> ids, string username) => Task.FromResult(0);
    }
}
