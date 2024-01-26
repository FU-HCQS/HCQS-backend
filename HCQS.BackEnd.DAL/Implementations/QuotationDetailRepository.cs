using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class QuotationDetailRepository : Repository<QuotationDetail>, IQuotationDetailRepository
    {
        public QuotationDetailRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}