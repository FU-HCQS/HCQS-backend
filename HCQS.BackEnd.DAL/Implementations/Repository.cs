using HCQS.BackEnd.DAL.Contracts;
using HCQS.BackEnd.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HCQS.BackEnd.DAL.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly HCQSDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(HCQSDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public Task<List<T>> GetAllDataByExpression(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includes != null)
            {
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
            }

            return query.ToListAsync();
        }

        public async Task<T> GetById(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> Insert(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<T> Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public async Task<T> DeleteById(object id)
        {
            T entityToDelete = await _dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
            }
            return entityToDelete;
        }

        public async Task<T> GetByExpression(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.SingleOrDefaultAsync(filter);
        }

        public async Task<IEnumerable<T>> InsertRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
            return entities;
        }

        public async Task<IEnumerable<T>> DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            return entities;
        }
    }
}