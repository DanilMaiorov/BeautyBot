using BeautyBot.src.BeautyBot.Core.Enums;
using LinqToDB.Mapping;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Models
{
    [Table("Appointments")]
    public class AppointmentModel
    {
        [Column("Id"), PrimaryKey]
        public Guid Id { get; set; }

        [Column("UserId"), NotNull]
        public Guid UserId { get; set; }

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(BeautyBotUserModel.UserId))]
        public BeautyBotUserModel User { get; set; }

        [Column("ProcedureId"), NotNull]
        public string ProcedureId { get; set; }

        [Association(ThisKey = nameof(ProcedureId), OtherKey = nameof(ProcedureModel.Id))]
        public ProcedureModel Procedure { get; set; }

        [Column("CreatedAt"), NotNull]
        public DateTime CreatedAt { get; set; }

        [Column("StateChangedAt"), NotNull]
        public DateTime StateChangedAt { get; set; }

        [Column("AppointmentDate"), NotNull]
        public DateTime AppointmentDate { get; set; }

        [Column("AppointmentDuration"), NotNull]
        public int AppointmentDuration { get; set; }

        [Column("State"), NotNull]
        public AppointmentState State { get; set; }
    }
}
