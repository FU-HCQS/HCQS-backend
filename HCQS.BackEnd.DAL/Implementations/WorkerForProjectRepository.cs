using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class WorkerForProjectRepository : Repository<WorkerForProject>, IWorkerForProjectRepository
    {
        public WorkerForProjectRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}