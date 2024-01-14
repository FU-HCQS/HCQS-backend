using HCQS.BackEnd.Common.Dto;
using HCQS.BackEnd.Service.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HCQS.BackEnd.API.Controllers
{
    [Route("role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("get-all-role")]
        public async Task<AppActionResult> GetAllRole()
        {
            return await _roleService.GetAllRole();
        }

        [HttpPost("assign-role-for-user")]
        public async Task<AppActionResult> AssignRoleForUser(string userId, string roleName)
        {
            return await _roleService.AssignRoleForUser(userId, roleName);
        }

        [HttpDelete("remove-role-for-user")]
        public async Task<AppActionResult> RemoveRoleForUser(string userId, string roleName)
        {
            return await _roleService.RemoveRoleForUser(userId, roleName);
        }
    }
}