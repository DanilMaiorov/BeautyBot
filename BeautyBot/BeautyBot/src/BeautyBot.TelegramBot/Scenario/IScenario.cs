using BeautyBot.src.BeautyBot.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public interface IScenario
    {
        bool CanHandle(ScenarioType scenario);
        Task<ScenarioResponse> HandleMessageAsync(ScenarioContext context, MessageData messageData, CancellationToken ct);
    }
}
