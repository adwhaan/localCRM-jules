using LocalCRM.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllAsync();
        Task<IEnumerable<CompanyDto>> GetDeletedAsync();
        Task<CompanyDto?> GetByIdAsync(int id);
        Task<CompanyDto> CreateAsync(CreateCompanyDto dto, string username);
        Task<bool> UpdateAsync(int id, UpdateCompanyDto dto, string username, DateTime updatedAt);
        Task<bool> DeleteAsync(int id, string username);
        Task<bool> RestoreAsync(int id, string username);
        Task<int> BulkDeleteAsync(IEnumerable<int> ids, string username);
        Task<int> BulkRestoreAsync(IEnumerable<int> ids, string username);
        Task<IEnumerable<CompanyDto>> SearchAsync(string query);
    }
}
