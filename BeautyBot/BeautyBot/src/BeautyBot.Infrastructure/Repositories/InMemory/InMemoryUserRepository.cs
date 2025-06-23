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



        //private readonly List<User> _users = new List<User>();
        //// ... реализация методов IUserRepository
        //public User? GetByTelegramId(long telegramUserId) => _users.FirstOrDefault(u => u.TelegramUserId == telegramUserId);
        //public User? GetById(Guid id) => _users.FirstOrDefault(u => u.Id == id);
        //public IReadOnlyList<User> GetAll() => _users.AsReadOnly();
        //public void Add(User entity) { _users.Add(entity); }
        //public void Update(User entity)
        //{
        //    var existing = _users.FirstOrDefault(u => u.Id == entity.Id);
        //    if (existing != null) { /* update properties */ }
        //}
        //public void Delete(Guid id) { _users.RemoveAll(u => u.Id == id); }


        //Task<BeautyBotUser?> GetUser(Guid userId, CancellationToken ct);
        //Task<BeautyBotUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct);
        //Task Add(BeautyBotUser user, CancellationToken ct);
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
