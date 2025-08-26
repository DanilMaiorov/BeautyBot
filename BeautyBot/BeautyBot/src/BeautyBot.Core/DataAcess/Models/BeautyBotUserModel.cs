using LinqToDB.Mapping;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Models
{

    [Table("BeautyBotUsers")]
    public class BeautyBotUserModel
    {
        [Column("UserId"), PrimaryKey]
        public Guid UserId { get; set; }

        [Column("TelegramUserId"), NotNull]
        public long TelegramUserId { get; set; }

        [Column("TelegramUserName"), NotNull]
        public string TelegramUserName { get; set; }

        [Column("RegisteredAt"), NotNull]
        public DateTime RegisteredAt { get; set; }
    }
}
