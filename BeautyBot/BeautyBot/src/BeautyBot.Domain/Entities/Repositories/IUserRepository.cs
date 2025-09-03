using BeautyBot.src.BeautyBot.Core.Interfaces;

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
