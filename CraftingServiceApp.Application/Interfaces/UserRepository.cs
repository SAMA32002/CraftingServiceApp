using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using System.Linq.Expressions;

namespace CraftingServiceApp.Application.Interfaces
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser GetById(string id)
        {
            return _context.Users.Find(id);
        }

        public IQueryable<ApplicationUser> GetAll()
        {
            return _context.Users;
        }

        public void Add(ApplicationUser user)
        {
            _context.Users.Add(user);
        }

        public void Update(ApplicationUser user)
        {
            _context.Users.Update(user);
        }

        public void Delete(ApplicationUser user)
        {
            _context.Users.Remove(user);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public IQueryable<ApplicationUser> Find(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return _context.Users.Where(predicate);
        }

    }

}
