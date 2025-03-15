
using CraftingServiceApp.Domain.Entities;
using System.Linq.Expressions;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll(); // Change return type from IEnumerable<T> to IQueryable<T>
        IQueryable<T> Find(Expression<Func<T, bool>> predicate); // Same for Find
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
        Task<T> GetByIdAsync(int id);
        Task<IQueryable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task SaveAsync();
    }

}
