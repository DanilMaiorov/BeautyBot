using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface IUserService
    {
        Task<BeautyBotUser> RegisterUser(BeautyBotUser user, CancellationToken ct);
        Task<BeautyBotUser?> GetUser(long telegramUserId, CancellationToken ct);
    }
}
