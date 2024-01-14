using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using Microsoft.AspNetCore.Identity;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class IdentityRoleRepository : Repository<IdentityRole>, IIdentityRoleRepository
    {
        public IdentityRoleRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}