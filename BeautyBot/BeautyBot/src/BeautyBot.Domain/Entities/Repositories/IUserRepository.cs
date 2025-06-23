using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Domain.Entities.Repositories
{
    /// <summary>
    /// Интерфейс, описывающий методы хранилища юзеров
    /// </summary>
    public interface IUserRepository : IRepository<BeautyBotUser>
    {
        Task<BeautyBotUser?> GetUser(Guid userId, CancellationToken ct);
        Task<BeautyBotUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct);
        Task Add(BeautyBotUser user, CancellationToken ct);
    }
}
