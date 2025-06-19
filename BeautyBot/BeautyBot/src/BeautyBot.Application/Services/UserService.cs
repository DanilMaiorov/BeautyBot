using BeautyBot.src.BeautyBot.Domain.Repositories;
using BeautyBot.src.BeautyBot.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //public User RegisterUser(long telegramUserId, string telegramUserName)
        //{
        //    var existingUser = _userRepository.GetByTelegramId(telegramUserId);
        //    if (existingUser != null) return existingUser;

        //    var newUser = new User(telegramUserId, telegramUserName);
        //    _userRepository.Add(newUser);
        //    return newUser;
        //}

        //public User? GetUser(long telegramUserId) => _userRepository.GetByTelegramId(telegramUserId);
    }
}
