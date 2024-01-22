using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class SupplierPriceDetailRepository : Repository<SupplierPriceDetail>, ISupplierPriceDetailRepository
    {
        public SupplierPriceDetailRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}
