using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public class ScenarioResponse
    {
        public ScenarioResponse(ScenarioResult result, Chat chat) 
        {
            Result = result;
            Chat = chat;
        }
        public ScenarioResult Result { get; set; }
        public Chat Chat { get; set; }
        public string Message { get; set; }
        public ReplyMarkup Keyboard { get; set; }
        public List<(string message, ReplyMarkup keyboard)> Messages { get; set; }
    }
}
