using System.Linq.Expressions;

namespace HCQS.BackEnd.DAL.Contracts
{
    public interface IRepository<T> where T : class
    {
        Task<IOrderedQueryable<T>> GetAll();

        Task<T> GetById(object id);

        Task<T> GetByExpression(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties);

        Task<IOrderedQueryable<T>> GetListByExpression(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties);

        Task<T> Insert(T entity);

        Task<IEnumerable<T>> InsertRange(IEnumerable<T> entities);

        Task<IEnumerable<T>> DeleteRange(IEnumerable<T> entities);

        Task Update(T entity);

        Task DeleteById(object id);
    }
}