using AutoMapper;
using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Entities;
using LocalCRM.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalCRM.Application.Services
{
    public class TagService : ITagService
    {
        private readonly IRepository<Tag> _repo;
        private readonly IMapper _mapper;
        public TagService(IRepository<Tag> repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
        public async Task<IEnumerable<TagDto>> GetAllAsync() => _mapper.Map<IEnumerable<TagDto>>(await _repo.GetAllAsync());
        public async Task<TagDto> CreateAsync(TagDto dto) { var item = _mapper.Map<Tag>(dto); _repo.Add(item); await _repo.SaveChangesAsync(); return _mapper.Map<TagDto>(item); }
        public async Task<bool> DeleteAsync(int id) { var item = await _repo.GetByIdAsync(id); if (item == null) return false; _repo.Remove(item); await _repo.SaveChangesAsync(); return true; }
    }

    public class SettingService : ISettingService
    {
        private readonly IRepository<Setting> _repo;
        private readonly IMapper _mapper;
        public SettingService(IRepository<Setting> repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
        public async Task<IEnumerable<SettingDto>> GetAllAsync() => _mapper.Map<IEnumerable<SettingDto>>(await _repo.GetAllAsync());
        public async Task<SettingDto?> GetByKeyAsync(string key) => _mapper.Map<SettingDto>(await _repo.Query().FirstOrDefaultAsync(s => s.SettingKey == key));
        public async Task<bool> UpdateAsync(string key, string value) { var item = await _repo.Query().FirstOrDefaultAsync(s => s.SettingKey == key); if (item == null) return false; item.SettingValue = value; await _repo.SaveChangesAsync(); return true; }
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<AuditLog> _repo;
        private readonly IMapper _mapper;
        public AuditLogService(IRepository<AuditLog> repo, IMapper mapper) { _repo = repo; _mapper = mapper; }
        public async Task<IEnumerable<AuditLogDto>> GetAllAsync() => _mapper.Map<IEnumerable<AuditLogDto>>(await _repo.GetAllAsync());
        public async Task<IEnumerable<AuditLogDto>> SearchAsync(string query) => _mapper.Map<IEnumerable<AuditLogDto>>(await _repo.Query().Where(l => l.Action.Contains(query) || l.EntityName.Contains(query) || l.Username.Contains(query)).ToListAsync());
    }

    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IRepository<Permission> _permissionRepo;
        private readonly IRepository<RolePermission> _rolePermissionRepo;
        private readonly IMapper _mapper;

        public RoleService(RoleManager<Role> roleManager, IRepository<Permission> permissionRepo, IRepository<RolePermission> rolePermissionRepo, IMapper mapper)
        {
            _roleManager = roleManager;
            _permissionRepo = permissionRepo;
            _rolePermissionRepo = rolePermissionRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>> GetRolesAsync() => _mapper.Map<IEnumerable<RoleDto>>(await _roleManager.Roles.ToListAsync());
        public async Task<IEnumerable<PermissionDto>> GetPermissionsAsync() => _mapper.Map<IEnumerable<PermissionDto>>(await _permissionRepo.GetAllAsync());
        public async Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId)
        {
            var perms = await _rolePermissionRepo.Query()
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission)
                .ToListAsync();
            return _mapper.Map<IEnumerable<PermissionDto>>(perms);
        }

        public async Task<bool> AddPermissionToRoleAsync(int roleId, int permissionId)
        {
            if (await _rolePermissionRepo.Query().AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)) return true;
            _rolePermissionRepo.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
            await _rolePermissionRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var rp = await _rolePermissionRepo.Query().FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (rp == null) return false;
            _rolePermissionRepo.Remove(rp);
            await _rolePermissionRepo.SaveChangesAsync();
            return true;
        }
    }
}
