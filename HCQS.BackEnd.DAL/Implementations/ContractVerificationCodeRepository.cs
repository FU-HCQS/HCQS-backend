using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class ContractVerificationCodeRepository : Repository<ContractVerificationCode>, IContractVerificationCodeRepository
    {
        public ContractVerificationCodeRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}
