using CraftingServiceApp.BLL.Mapper;
using CraftingServiceApp.Common.DTOs;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Domain.Enums;

namespace CraftingServiceApp.Application.Interfaces
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserDTO GetUserById(string userId)
        {
            var user = _userRepository.GetById(userId);
            return user != null ? UserMapper.MapToDTO(user) : null;
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            return _userRepository.GetAll().Select(UserMapper.MapToDTO);
        }

        public void BanUser(string userId)
        {
            var user = _userRepository.GetById(userId);
            if (user != null)
            {
                user.IsBanned = true;
                _userRepository.Update(user);
                _userRepository.SaveChanges();
            }
        }

        public void UnbanUser(string userId)
        {
            var user = _userRepository.GetById(userId);
            if (user != null)
            {
                user.IsBanned = false;
                _userRepository.Update(user);
                _userRepository.SaveChanges();
            }
        }

        public IEnumerable<UserDTO> GetCrafters()
        {
            return _userRepository.Find(u => u.Role == UserRole.Crafter).Select(UserMapper.MapToDTO);
        }
    }

}
