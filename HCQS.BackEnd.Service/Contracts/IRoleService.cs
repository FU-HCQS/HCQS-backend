using HCQS.BackEnd.Common.Dto;
using Microsoft.AspNetCore.Identity;

namespace HCQS.BackEnd.Service.Contracts
{
    public interface IRoleService
    {


        Task<AppActionResult> AssignRoleForUser(string userId);

        Task<AppActionResult> RemoveRoleForUser(string userId);

        Task<AppActionResult> GetAllRole();
    }
}