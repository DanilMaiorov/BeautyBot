using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using LinqToDB.Mapping;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Models
{

    [Table("Appointments")]
    public class AppointmentModel
    {
        [Column("Id"), PrimaryKey]
        public Guid Id { get; set; }

        [Column("UserId")]
        public Guid UserId { get; set; }

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(BeautyBotUserModel.UserId))]
        public BeautyBotUserModel User { get; set; } = null!;

        [Column("ProcedureId")]
        public Guid ProcedureId { get; set; }

        [Column("ProcedureName"), NotNull] // Храним название процедуры для фабрики
        public string ProcedureName { get; set; } = string.Empty;

        [Column("ProcedureBaseType"), NotNull] // Базовый тип: Manicure или Pedicure
        public string ProcedureBaseType { get; set; } = string.Empty;

        [Column("CreatedAt"), NotNull]
        public DateTime CreatedAt { get; set; }

        [Column("StateChangedAt"), NotNull]
        public DateTime StateChangedAt { get; set; }

        [Column("AppointmentDate"), NotNull]
        public DateTime AppointmentDate { get; set; }

        [Column("AppointmentDuration"), NotNull]
        public int AppointmentDuration { get; set; }

        [Column("AppointmentState"), NotNull]
        public AppointmentState State { get; set; }
    }
}
