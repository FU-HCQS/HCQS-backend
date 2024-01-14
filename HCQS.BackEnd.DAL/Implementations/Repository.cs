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

        public async Task<IOrderedQueryable<T>> GetAll()
        {
            return (IOrderedQueryable<T>)_dbSet.AsNoTracking();
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

        public async Task Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteById(object id)
        {
            T entityToDelete = await _dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                _dbSet.Remove(entityToDelete);
            }
        }

        public async Task<IOrderedQueryable<T>> GetListByExpression(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbSet.AsQueryable();

            // Apply eager loading
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (filter == null && includeProperties.Length > 0)
            {
                return (IOrderedQueryable<T>)await query.ToListAsync();
            }

            return (IOrderedQueryable<T>)query.Where(filter);
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