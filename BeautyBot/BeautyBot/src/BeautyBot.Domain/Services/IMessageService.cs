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
        Task<Message> SendMessage(Chat chat, string message, ReplyMarkup replyMarkup, CancellationToken ct);
        Task<Message> SendMultiMessage();
        Task<Message> EditMessage();
        Task<Message> DeleteMessage();
    }
}
