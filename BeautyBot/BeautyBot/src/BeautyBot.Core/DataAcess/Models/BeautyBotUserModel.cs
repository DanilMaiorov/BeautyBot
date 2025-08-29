using LinqToDB.Mapping;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Models
{
    [Table("Users")]
    public class BeautyBotUserModel
    {
        [Column("Id"), PrimaryKey]
        public Guid UserId { get; set; }

        [Column("TelegramId"), NotNull]
        public long TelegramUserId { get; set; }

        [Column("UserName"), NotNull]
        public string? TelegramUserName { get; set; }

        [Column("FirstName"), NotNull]
        public string? TelegramUserFirstName { get; set; }

        [Column("LastName"), NotNull]
        public string? TelegramUserLastName { get; set; }

        [Column("RegisteredAt"), NotNull]
        public DateTime RegisteredAt { get; set; }
    }
}
