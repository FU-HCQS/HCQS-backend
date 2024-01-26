using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;


namespace HCQS.BackEnd.DAL.Implementations
{
    public class MaterialRepository : Repository<Material>, IMaterialRepository
    {
        public MaterialRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}

