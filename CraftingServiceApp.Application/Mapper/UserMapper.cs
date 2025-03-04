using CraftingServiceApp.Common.DTOs;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingServiceApp.BLL.Mapper
{
    public static class UserMapper
    {
        public static UserDTO MapToDTO(ApplicationUser user)
        {
            return new UserDTO
            {
                Id = user.Id,  // ✅ IdentityUser uses string ID
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = (int)user.Role,  // ✅ Convert Enum to int
                IsBanned = user.IsBanned
            };
        }

        public static ApplicationUser MapToEntity(UserDTO dto)
        {
            return new ApplicationUser
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Role = (UserRole)dto.Role,  // ✅ Convert int back to Enum
                IsBanned = dto.IsBanned
            };
        }
    }
}
