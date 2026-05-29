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
    public class InteractionService : IInteractionService
    {
        private readonly IRepository<Interaction> _repo;
        private readonly IMapper _mapper;

        public InteractionService(IRepository<Interaction> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InteractionDto>> GetAllAsync() => _mapper.Map<IEnumerable<InteractionDto>>(await _repo.GetAllAsync());
        public async Task<InteractionDto?> GetByIdAsync(int id) => _mapper.Map<InteractionDto>(await _repo.GetByIdAsync(id));
        public async Task<InteractionDto> CreateAsync(CreateInteractionDto dto, string username) { return new InteractionDto(); }
        public async Task<bool> UpdateAsync(int id, UpdateInteractionDto dto, string username, DateTime updatedAt) => false;
        public async Task<bool> DeleteAsync(int id, string username) => false;
        public async Task<bool> RestoreAsync(int id, string username) => false;
        public Task<int> BulkDeleteAsync(IEnumerable<int> ids, string username) => Task.FromResult(0);
        public Task<int> BulkRestoreAsync(IEnumerable<int> ids, string username) => Task.FromResult(0);
    }
}
