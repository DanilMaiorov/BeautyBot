using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public BeautyBotUser User { get; set; }
        public IProcedure Procedure { get; set; } // храню интерфейс или конкретный класс
        public DateTime CreatedAt { get; set; }
        public DateTime StateChangedAt { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int AppointmentDuration { get; set; }
        public AppointmentState State { get; set; }

        public Appointment(BeautyBotUser user, IProcedure procedure, DateOnly appointmentDate, TimeOnly appointmentTime)
        {
            Id = Guid.NewGuid();
            User = user;
            Procedure = procedure;
            CreatedAt = DateTime.Now;
            StateChangedAt = DateTime.Now; 
            AppointmentDate = appointmentDate.ToDateTime(appointmentTime);
            AppointmentDuration = procedure.Duration;
            State = AppointmentState.Active;
        }
    }
}
