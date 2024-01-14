using HCQS.BackEnd.Common.Dto;
using Microsoft.AspNetCore.Identity;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IRoleService
    {
        Task<AppActionResult> CreateRole(string roleName);

        Task<AppActionResult> UpdateRole(IdentityRole role);

        Task<AppActionResult> AssignRoleForUser(string userId, string roleName);

        Task<AppActionResult> RemoveRoleForUser(string userId, string roleName);

        Task<AppActionResult> GetAllRole();
    }
}