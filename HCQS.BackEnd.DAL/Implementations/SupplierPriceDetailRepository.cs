using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class SupplierPriceDetailRepository : Repository<SupplierPriceDetail>, ISupplierPriceDetailRepository
    {
        public SupplierPriceDetailRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}