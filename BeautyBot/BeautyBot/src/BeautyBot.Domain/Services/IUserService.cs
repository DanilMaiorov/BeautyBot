using BeautyBot.src.BeautyBot.Domain.Entities;
using Telegram.Bot.Types;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface IUserService
    {
        Task<BeautyBotUser> RegisterUser(long telegramId, string userName, string userFirstName, string userLastName, CancellationToken ct);
        Task<BeautyBotUser?> GetUser(long telegramUserId, CancellationToken ct);
    }
}
