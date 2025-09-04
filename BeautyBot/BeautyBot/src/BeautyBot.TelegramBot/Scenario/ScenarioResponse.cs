using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public class ScenarioResponse
    {
        public ScenarioResult Result { get; set; }
        public string Message { get; set; }
        public long Chat {  get; set; }
        public List<string> Messages { get; set; }
        public ReplyMarkup Keyboard { get; set; }
        public List<ReplyMarkup> Keyboards { get; set; }
    }
}
