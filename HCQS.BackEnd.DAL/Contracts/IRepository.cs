using System.Linq.Expressions;

namespace HCQS.BackEnd.DAL.Contracts
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllDataByExpression(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

        Task<T> GetById(object id);

        Task<T> GetByExpression(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties);

        Task<T> Insert(T entity);

        Task<IEnumerable<T>> InsertRange(IEnumerable<T> entities);

        Task<IEnumerable<T>> DeleteRange(IEnumerable<T> entities);

        Task<T> Update(T entity);

        Task<T> DeleteById(object id);
    }
}