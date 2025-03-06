
using CraftingServiceApp.Domain.Entities;
using System.Linq.Expressions;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task SaveAsync();
    }
}
