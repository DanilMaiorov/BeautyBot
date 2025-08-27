namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    /// <summary>
    /// Класс описания юзера(клиента)
    /// </summary>
    public class BeautyBotUser
    {
        public Guid UserId { get; set; }
        public long TelegramUserId { get; set; }
        public string? TelegramUserName { get; set; }
        public string? TelegramUserFirstName { get; set; }
        public string? TelegramUserLastName { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
