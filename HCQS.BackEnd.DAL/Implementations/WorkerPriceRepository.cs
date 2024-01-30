using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class WorkerPriceRepository : Repository<WorkerPrice>, IWorkerPriceRepository
    {
        public WorkerPriceRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}