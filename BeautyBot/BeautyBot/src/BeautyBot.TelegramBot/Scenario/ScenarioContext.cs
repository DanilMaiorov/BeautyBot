namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public class ScenarioContext
    {
        public ScenarioContext(ScenarioType scenario, long userId)
        {
            CurrentScenario = scenario;
            UserId = userId;
        }
        public long UserId { get; }
        public ScenarioType CurrentScenario { get; set; }
        public ScenarioResponse LastResponse { get; set; }
        public Stack<string> DataHistory { get; set; } = new Stack<string>();
        public string? CurrentStep { get; set; }
        public Dictionary<string, object> Data { get; set; } = [];
    }
}
