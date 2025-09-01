using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<BeautyBotUser> UsersList = [];

        public async Task<BeautyBotUser?> GetUser(Guid userId, CancellationToken ct)
        {
            var user = UsersList.FirstOrDefault(x => x.UserId == userId);

            //сделаю искусственную задержку для асинхронности
            await Task.Delay(1, ct);

            return user;
        }

        public async Task<BeautyBotUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            var user = UsersList.FirstOrDefault(x => x.TelegramUserId == telegramUserId);

            //сделаю искусственную задержку для асинхронности
            await Task.Delay(1, ct);

            return user;
        }

        public async Task Add(BeautyBotUser user, CancellationToken ct)
        {
            UsersList.Add(user);

            //сделаю искусственную задержку для асинхронности
            await Task.Delay(1, ct);
        }
    }
}
