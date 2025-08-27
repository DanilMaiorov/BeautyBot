using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface IUserService
    {
        Task<BeautyBotUser> RegisterUser(long telegramUserId, string telegramUserName, string telegramUserFirstName, string telegramUserLastName, CancellationToken ct);
        Task<BeautyBotUser?> GetUser(long telegramUserId, CancellationToken ct);
    }
}
