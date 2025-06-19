using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Domain.Repositories
{
    /// <summary>
    /// Интерфейс, описывающий методы хранилища юзеров
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        //Не асинхронные
        User? GetUser(Guid userId, CancellationToken ct);
        User? GetUserByTelegramUserId(long telegramUserId, CancellationToken ct);
        void Add(User user, CancellationToken ct);


        //Асинхронные
        //Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct);
        //Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct);
        //Task Add(ToDoUser user, CancellationToken ct);
    }
}
