using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class CreateAppointmentTemplate
    {
        public IProcedure Procedure { get; set; } // храню интерфейс или конкретный класс
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }

        public CreateAppointmentTemplate(IProcedure procedure, DateOnly appointmentDate = default, TimeOnly appointmentTime = default)
        {
            Procedure = procedure;
            AppointmentDate = appointmentDate;
            AppointmentTime = appointmentTime;
        }
    }
}
