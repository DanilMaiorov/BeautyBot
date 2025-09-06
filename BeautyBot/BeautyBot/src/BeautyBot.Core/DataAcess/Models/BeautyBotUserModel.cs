using BeautyBot.src.BeautyBot.Core.Enums;
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

        [Column("UserName")]
        public string? TelegramUserName { get; set; }

        [Column("FirstName")]
        public string? TelegramUserFirstName { get; set; }

        [Column("LastName")]
        public string? TelegramUserLastName { get; set; }

        [Column("RegisteredAt"), NotNull]
        public DateTime RegisteredAt { get; set; }

        [Column("UserRole"), NotNull]
        public UserRole Role { get; set; }
    }
}
