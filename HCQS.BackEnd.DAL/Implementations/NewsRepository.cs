using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using HCQS.BackEnd.DAL.Models;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        public NewsRepository(HCQSDbContext context) : base(context)
        {
        }
    }
}