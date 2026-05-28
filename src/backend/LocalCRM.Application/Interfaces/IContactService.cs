using LocalCRM.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDto>> GetAllAsync();
        Task<ContactDto?> GetByIdAsync(int id);
        Task<ContactDto> CreateAsync(CreateContactDto dto, string username);
        Task<bool> UpdateAsync(int id, UpdateContactDto dto, string username, DateTime updatedAt);
        Task<bool> DeleteAsync(int id, string username);
        Task<bool> RestoreAsync(int id, string username);
        Task<int> BulkDeleteAsync(IEnumerable<int> ids, string username);
        Task<int> BulkRestoreAsync(IEnumerable<int> ids, string username);
    }
}
