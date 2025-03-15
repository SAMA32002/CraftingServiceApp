using CraftingServiceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IUserRepository
    {
        ApplicationUser GetById(string id);
        IQueryable<ApplicationUser> GetAll();
        void Add(ApplicationUser user);
        void Update(ApplicationUser user);
        void Delete(ApplicationUser user);
        void SaveChanges();
        IQueryable<ApplicationUser> Find(Expression<Func<ApplicationUser, bool>> predicate);

    }

}
