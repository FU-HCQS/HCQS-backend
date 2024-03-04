using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class ContractVerificationCodeRepository : Repository<ContractVerificationCode>, IContractVerificationCodeRepository
    {
        public ContractVerificationCodeRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}