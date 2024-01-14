using Microsoft.AspNetCore.Identity;

namespace HCQS.BackEnd.DAL.Contracts
{
    public interface IUserRoleRepository : IRepository<IdentityUserRole<string>>
    {
    }
}