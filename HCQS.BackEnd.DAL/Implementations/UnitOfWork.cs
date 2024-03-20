using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly HCQSDbContext _db;

        public UnitOfWork(HCQSDbContext db)
        {
            _db = db;
        }

        public void Dispose()
        {
           GC.SuppressFinalize(this);
        }

        public async Task SaveChangeAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}