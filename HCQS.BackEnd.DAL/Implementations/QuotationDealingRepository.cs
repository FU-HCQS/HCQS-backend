using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class QuotationDealingRepository : Repository<QuotationDealing>, IQuotationDealingRepository
    {
        public QuotationDealingRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}