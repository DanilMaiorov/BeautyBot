using Telegram.Bot.Types;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class MessageData
    {
        public MessageData(Chat chat, string userInput, int messageId, BeautyBotUser user) 
        { 
            Chat = chat;
            UserInput = userInput;
            MessageId = messageId;  
            User = user ?? null;
            TelegramUserId = User?.TelegramUserId ?? 0;
        }

        public Chat Chat { get; }
        public string UserInput { get; }
        public int MessageId { get; }
        public BeautyBotUser? User { get; }
        public long TelegramUserId { get; }
    }
}
