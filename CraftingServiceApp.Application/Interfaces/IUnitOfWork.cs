
using System.Linq.Expressions;

namespace CraftingServiceApp.BLL.Interfaces
{
    public interface IUnitOfWork<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CompleteAsync();
    }
}
