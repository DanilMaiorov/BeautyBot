using BeautyBot.src.BeautyBot.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly ITelegramBotClient _botClient;

        public MessageService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task SendMessage(Chat chat, string message, ReplyMarkup replyMarkup, CancellationToken ct)
        {
            await _botClient.SendMessage(chat, message, replyMarkup: replyMarkup, cancellationToken: ct);
        }
        
        public async Task SendMultiMessage(Chat chat, List<(string message, ReplyMarkup keyboard)> messages, CancellationToken ct)
        {
            foreach (var message in messages)
                await SendMessage(chat, message.message, message.keyboard, ct);
        }

        public async Task DeleteMessage(Chat chat, int messageId, CancellationToken ct)
        {
            await _botClient.DeleteMessage(chatId: chat, messageId: messageId, cancellationToken: ct);
        }

        public async Task DeleteMultiMessage(Chat chat, List<int> messageIds, CancellationToken ct)
        {
            foreach (var messageId in messageIds)
                await DeleteMessage(chat, messageId, ct);
        }

        public async Task EditMessage(Chat chat, int messageId, InlineKeyboardMarkup keyboard, CancellationToken ct)
        {
            await _botClient.EditMessageReplyMarkup(chat, messageId, replyMarkup: keyboard, cancellationToken: ct);
        }

        public async Task EditMessageText(Chat chat, int messageId, string text, InlineKeyboardMarkup keyboard, CancellationToken ct)
        {
            await _botClient.EditMessageText(chat, messageId, text, replyMarkup: keyboard, cancellationToken: ct);
        }
    }
}