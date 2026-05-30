using LocalCRM.Application.DTOs;
using LocalCRM.Application.Interfaces;
using LocalCRM.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalCRM.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagsController(ITagService tagService) { _tagService = tagService; }
        [HttpGet] public async Task<ActionResult<IEnumerable<TagDto>>> GetAll() => Ok(await _tagService.GetAllAsync());

        [HttpPost]
        [Authorize(Policy = Permissions.SettingsManage)]
        public async Task<ActionResult<TagDto>> Create(TagDto dto) => Ok(await _tagService.CreateAsync(dto));

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.SettingsManage)]
        public async Task<IActionResult> Delete(int id) => await _tagService.DeleteAsync(id) ? NoContent() : NotFound();
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _settingService;
        public SettingsController(ISettingService settingService) { _settingService = settingService; }

        [HttpGet]
        [Authorize(Policy = Permissions.SettingsManage)]
        public async Task<ActionResult<IEnumerable<SettingDto>>> GetAll() => Ok(await _settingService.GetAllAsync());

        [HttpGet("{key}")] public async Task<ActionResult<SettingDto>> Get(string key) { var s = await _settingService.GetByKeyAsync(key); return s != null ? Ok(s) : NotFound(); }

        [HttpPut("{key}")]
        [Authorize(Policy = Permissions.SettingsManage)]
        public async Task<IActionResult> Update(string key, [FromBody] string value) => await _settingService.UpdateAsync(key, value) ? NoContent() : NotFound();
    }

    [ApiController]
    [Route("api/audit-logs")]
    [Authorize(Policy = Permissions.AuditRead)]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        public AuditLogsController(IAuditLogService auditLogService) { _auditLogService = auditLogService; }
        [HttpGet] public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAll() => Ok(await _auditLogService.GetAllAsync());
        [HttpPost("search")] public async Task<ActionResult<IEnumerable<AuditLogDto>>> Search([FromQuery] string q) => Ok(await _auditLogService.SearchAsync(q));
    }

    [ApiController]
    [Route("api/roles")]
    [Authorize(Policy = Permissions.RolesAssignPermissions)]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService) { _roleService = roleService; }
        [HttpGet] public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles() => Ok(await _roleService.GetRolesAsync());
        [HttpGet("{id}/permissions")] public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions(int id) => Ok(await _roleService.GetRolePermissionsAsync(id));
        [HttpPost("{id}/permissions")] public async Task<IActionResult> AddPermission(int id, [FromBody] int permissionId) => await _roleService.AddPermissionToRoleAsync(id, permissionId) ? Ok() : BadRequest();
        [HttpDelete("{id}/permissions/{permissionId}")] public async Task<IActionResult> RemovePermission(int id, int permissionId) => await _roleService.RemovePermissionFromRoleAsync(id, permissionId) ? NoContent() : NotFound();
    }

    [ApiController]
    [Route("api/permissions")]
    [Authorize(Policy = Permissions.RolesAssignPermissions)]
    public class PermissionsController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public PermissionsController(IRoleService roleService) { _roleService = roleService; }
        [HttpGet] public async Task<ActionResult<IEnumerable<PermissionDto>>> Get() => Ok(await _roleService.GetPermissionsAsync());
    }
}
