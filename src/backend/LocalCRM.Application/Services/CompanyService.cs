using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Enums;
using LocalCRM.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalCRM.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepository<Company> _repo;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public CompanyService(IRepository<Company> repo, IMapper mapper, IAuditService auditService)
        {
            _repo = repo;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            var companies = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            var company = await _repo.GetByIdAsync(id);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto, string username)
        {
            var company = _mapper.Map<Company>(dto);
            company.CreatedBy = username;
            company.CreatedAt = DateTime.UtcNow;
            _repo.Add(company);
            await _repo.SaveChangesAsync();
            await _auditService.LogAsync("companies", company.CompanyId, ActionTypes.Create, null, username);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCompanyDto dto, string username, DateTime updatedAt)
        {
            var company = await _repo.GetByIdAsync(id);
            if (company == null || (company.UpdatedAt != null && company.UpdatedAt != updatedAt)) return false;
            _mapper.Map(dto, company);
            company.UpdatedBy = username;
            company.UpdatedAt = DateTime.UtcNow;
            await _repo.SaveChangesAsync();
            await _auditService.LogAsync("companies", id, ActionTypes.Update, null, username);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string username)
        {
            var company = await _repo.GetByIdAsync(id);
            if (company == null) return false;
            company.IsDeleted = true;
            company.DeletedAt = DateTime.UtcNow;
            company.UpdatedBy = username;
            await _repo.SaveChangesAsync();
            await _auditService.LogAsync("companies", id, ActionTypes.SoftDelete, null, username);
            return true;
        }

        public async Task<bool> RestoreAsync(int id, string username)
        {
            // Requires custom query to include deleted
            return false; // TODO: Implement restore via repo query
        }

        public Task<int> BulkDeleteAsync(IEnumerable<int> ids, string username) => Task.FromResult(0);
        public Task<int> BulkRestoreAsync(IEnumerable<int> ids, string username) => Task.FromResult(0);
    }
}
