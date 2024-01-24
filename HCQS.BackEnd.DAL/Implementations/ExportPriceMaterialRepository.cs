using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class ExportPriceMaterialRepository : Repository<ExportPriceMaterial>, IExportPriceMaterialRepository
    {
        public ExportPriceMaterialRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}