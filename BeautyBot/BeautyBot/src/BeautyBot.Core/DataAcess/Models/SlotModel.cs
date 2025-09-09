using LinqToDB.Mapping;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Models
{
    [Table("Slots")]
    public class SlotModel
    {
        [Column("Date"), PrimaryKey]
        public DateTime Date { get; set; }

        [Column("StartTime"), PrimaryKey]
        public DateTime StartTime { get; set; }

        [Column("Duration"), NotNull]
        public int Duration { get; set; }

        [Column("AppointmentId"), Nullable]
        public Guid? AppointmentId { get; set; }
    }
}
