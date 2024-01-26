using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class QuotationRepository : Repository<Quotation>, IQuotationRepository
    {
        public QuotationRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}