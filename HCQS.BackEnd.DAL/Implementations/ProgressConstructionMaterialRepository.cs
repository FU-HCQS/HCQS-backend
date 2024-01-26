using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class ProgressConstructionMaterialRepository : Repository<ProgressConstructionMaterial>, IProgressConstructionMaterialRepository
    {
        public ProgressConstructionMaterialRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}