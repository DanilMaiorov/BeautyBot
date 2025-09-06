using BeautyBot.src.BeautyBot.Domain.Entities;
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


        public async Task<Message> SendMessage(Chat chat, string message, ReplyMarkup replyMarkup, CancellationToken ct)
        {
            return await _botClient.SendMessage(chat, message, replyMarkup: replyMarkup, cancellationToken: ct);
        }

        public Task<Message> SendMultiMessage()
        {
            throw new NotImplementedException();
        }







        public Task<Message> DeleteMessage()
        {
            throw new NotImplementedException();
        }

        public Task<Message> EditMessage()
        {
            throw new NotImplementedException();
        }
    }
}
