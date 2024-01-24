using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class QuotationDetailRepository : Repository<QuotationDetail>, IQuotationDetailRepository
    {
        public QuotationDetailRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}
