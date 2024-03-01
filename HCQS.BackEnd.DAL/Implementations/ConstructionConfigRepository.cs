using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class ConstructionConfigRepository : Repository<ConstructionConfig>, IConstructionConfigRepository
    {
        public ConstructionConfigRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}