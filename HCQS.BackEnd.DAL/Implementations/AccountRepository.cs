using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}