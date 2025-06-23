using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface IUserService
    {
        Task<BeautyBotUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct);
        Task<BeautyBotUser?> GetUser(long telegramUserId, CancellationToken ct);
    }
}
