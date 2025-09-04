using Telegram.Bot;
using Telegram.Bot.Types;

namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public interface IScenario
    {
        bool CanHandle(ScenarioType scenario);
        Task<ScenarioResponse> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct);
    }
}
