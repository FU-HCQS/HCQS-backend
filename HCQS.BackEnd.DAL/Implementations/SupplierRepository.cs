using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}