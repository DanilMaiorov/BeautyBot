namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    /// <summary>
    /// Класс описания юзера(клиента)
    /// </summary>
    public class User
    {
        public Guid UserId { get; init; }
        public long TelegramUserId { get; init; }
        public string TelegramUserName { get; init; }
        public DateTime RegisteredAt { get; init; }
    }
}
