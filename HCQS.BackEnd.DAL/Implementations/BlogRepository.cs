using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        public BlogRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}