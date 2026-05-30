using LocalCRM.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces
{
    public interface IEngagementService
    {
        Task<IEnumerable<EngagementDto>> GetAllAsync();
        Task<EngagementDto?> GetByIdAsync(int id);
        Task<EngagementDto> CreateAsync(CreateEngagementDto dto, string username);
        Task<bool> UpdateAsync(int id, UpdateEngagementDto dto, string username, DateTime updatedAt);
        Task<bool> DeleteAsync(int id, string username);
        Task<bool> RestoreAsync(int id, string username);
        Task<IEnumerable<EngagementDto>> SearchAsync(string query);
    }

    public interface INoteService
    {
        Task<IEnumerable<NoteDto>> GetAllAsync(string username, bool isElevated);
        Task<NoteDto?> GetByIdAsync(int id, string username, bool isElevated);
        Task<NoteDto> CreateAsync(CreateNoteDto dto, string username);
        Task<bool> UpdateAsync(int id, UpdateNoteDto dto, string username, DateTime updatedAt);
        Task<bool> DeleteAsync(int id, string username);
        Task<bool> RestoreAsync(int id, string username);
        Task<IEnumerable<NoteDto>> SearchAsync(string query, string username, bool isElevated);
    }

    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDto>> GetAllAsync(string username, bool isElevated);
        Task<DocumentDto?> GetByIdAsync(int id, string username, bool isElevated);
        Task<DocumentDto> CreateAsync(CreateDocumentDto dto, string username);
        Task<bool> UpdateAsync(int id, UpdateDocumentDto dto, string username, DateTime updatedAt);
        Task<bool> DeleteAsync(int id, string username);
        Task<bool> RestoreAsync(int id, string username);
        Task<IEnumerable<DocumentDto>> SearchAsync(string query, string username, bool isElevated);
    }

    public interface IExportService
    {
        Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data);
    }
}
