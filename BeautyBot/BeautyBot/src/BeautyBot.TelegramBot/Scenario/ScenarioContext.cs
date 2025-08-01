namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public class ScenarioContext
    {
        public long UserId { get; }
        public ScenarioType CurrentScenario { get; set; }
        public string? CurrentStep { get; set; }
        public Dictionary<string, object> Data { get; set; } = [];
        public ScenarioContext(ScenarioType scenario, long userId)
        {
            CurrentScenario = scenario;
            UserId = userId;
        }
    }
}
