using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class InMemoryUserRepository : IUserRepository
    {
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
        public void Add(User user, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public User? GetUser(Guid userId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public User? GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
