using LocalCRM.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<bool> DisableAsync(int id, string adminUsername);
        Task<bool> EnableAsync(int id, string adminUsername);
        Task<bool> UpdateRoleAsync(int id, string roleName, string adminUsername);
    }
}
