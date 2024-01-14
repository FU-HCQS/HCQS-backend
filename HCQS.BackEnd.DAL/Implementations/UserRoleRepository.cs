using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using Microsoft.AspNetCore.Identity;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class UserRoleRepository : Repository<IdentityUserRole<string>>, IUserRoleRepository
    {
        public UserRoleRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}