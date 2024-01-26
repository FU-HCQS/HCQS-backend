using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        public ContractRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}