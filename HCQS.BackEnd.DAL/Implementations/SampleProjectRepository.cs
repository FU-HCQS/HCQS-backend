using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class SampleProjectRepository : Repository<SampleProject>, ISampleProjectRepository
    {
        public SampleProjectRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}