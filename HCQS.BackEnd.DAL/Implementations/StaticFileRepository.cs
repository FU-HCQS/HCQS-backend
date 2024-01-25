using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class StaticFileRepository : Repository<StaticFile>, IStaticFileRepository
    {
        public StaticFileRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}