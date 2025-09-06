using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface IMessageService
    {
        Task SendMessage(Chat chat, string message, ReplyMarkup replyMarkup, CancellationToken ct);
        Task SendMultiMessage(Chat chat, List<(string message, ReplyMarkup keyboard)> messages, CancellationToken ct);
        Task DeleteMessage(Chat chat, int messageId, CancellationToken ct);
        Task DeleteMultiMessage(Chat chat, List<int> messageIds, CancellationToken ct);
        Task EditMessage(Chat chat, int messageId, InlineKeyboardMarkup keyboard, CancellationToken ct);
    }
}
