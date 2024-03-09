using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class ConstructionConfigValueRepository : Repository<ConstructionConfigValue>, IConstructionConfigValueRepository
    {
        public ConstructionConfigValueRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}