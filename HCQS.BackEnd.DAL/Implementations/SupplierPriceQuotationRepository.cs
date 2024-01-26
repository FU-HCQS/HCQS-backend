using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class SupplierPriceQuotationRepository : Repository<SupplierPriceQuotation>, ISupplierPriceQuotationRepository
    {
        public SupplierPriceQuotationRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}