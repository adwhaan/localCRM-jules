using LocalCRM.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces
{
    public interface IInteractionService
    {
        Task<IEnumerable<InteractionDto>> GetAllAsync();
        Task<IEnumerable<InteractionDto>> GetDeletedAsync();
        Task<InteractionDto?> GetByIdAsync(int id);
        Task<InteractionDto> CreateAsync(CreateInteractionDto dto, string username);
        Task<bool> UpdateAsync(int id, UpdateInteractionDto dto, string username, DateTime updatedAt);
        Task<bool> DeleteAsync(int id, string username);
        Task<bool> RestoreAsync(int id, string username);
        Task<int> BulkDeleteAsync(IEnumerable<int> ids, string username);
        Task<int> BulkRestoreAsync(IEnumerable<int> ids, string username);
        Task<IEnumerable<InteractionDto>> SearchAsync(string query);
    }
}
