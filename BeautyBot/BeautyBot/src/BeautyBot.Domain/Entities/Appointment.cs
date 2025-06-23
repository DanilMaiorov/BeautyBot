using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public IProcedure Procedure { get; set; } // храню интерфейс или конкретный класс
        public DateTime CreatedAt { get; set; }
        public DateTime StateChangedAt { get; set; }
        public DateTime AppointmentTime { get; set; }
        public AppointmentState State { get; set; } // Active, Completed, Cancelled

        public Appointment(Guid userId, IProcedure procedure, DateTime appointmentTime)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Procedure = procedure;
            CreatedAt = DateTime.Now;
            StateChangedAt = DateTime.Now; 
            AppointmentTime = appointmentTime;
            State = AppointmentState.Active;
        }
    }
}
