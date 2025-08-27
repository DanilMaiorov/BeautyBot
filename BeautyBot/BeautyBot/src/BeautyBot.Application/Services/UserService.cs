using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Entities.Repositories;
using BeautyBot.src.BeautyBot.Domain.Services;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<BeautyBotUser?> GetUser(long telegramUserId, CancellationToken ct)
        {
            var user = await _userRepository.GetUserByTelegramUserId(telegramUserId, ct);

            return user?.UserId != null
                ? await _userRepository.GetUser(user.UserId, ct)
                : null;
        }

        /// <summary>
        /// Метод регистрации нового юзера и добавление его в список
        /// </summary>
        /// <param name="telegramUserId">Телеграм ID</param>
        /// <param name="telegramUserName">Имя в Телеграм</param>
        /// <param name="ct"></param>
        /// <returns>BeautyBotUser</returns>
        public async Task<BeautyBotUser> RegisterUser(long userId, string userName, string firstName, string lastName, CancellationToken ct)
        {
            var newUser = new BeautyBotUser
            {
                UserId = Guid.NewGuid(),
                TelegramUserId = userId,
                TelegramUserName = userName,
                TelegramUserFirstName = firstName,
                TelegramUserLastName = lastName,
                RegisteredAt = DateTime.Now
            };

            await _userRepository.Add(newUser, ct);

            return newUser;
        }
    }
}