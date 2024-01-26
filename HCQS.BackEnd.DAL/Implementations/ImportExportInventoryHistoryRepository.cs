using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class ImportExportInventoryHistoryRepository : Repository<ImportExportInventoryHistory>, IImportExportInventoryHistoryRepository
    {
        public ImportExportInventoryHistoryRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}