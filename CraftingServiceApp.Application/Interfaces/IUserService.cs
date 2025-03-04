using CraftingServiceApp.Common.DTOs;
using CraftingServiceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingServiceApp.Application.Interfaces
{
    public interface IUserService
    {
        UserDTO GetUserById(string userId);
        IEnumerable<UserDTO> GetAllUsers();
        void BanUser(string userId);
        void UnbanUser(string userId);
        IEnumerable<UserDTO> GetCrafters();
    }

}
