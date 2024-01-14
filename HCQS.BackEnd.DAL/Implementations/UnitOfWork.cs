using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HCQSDbContext _db;

        public UnitOfWork(HCQSDbContext db)
        {
            _db = db;
        }

        public async Task SaveChangeAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}