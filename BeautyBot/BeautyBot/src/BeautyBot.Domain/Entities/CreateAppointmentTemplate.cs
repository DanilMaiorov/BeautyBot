using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class CreateAppointmentTemplate
    {
        public IProcedure Procedure { get; set; } // храню интерфейс или конкретный класс
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentTime { get; set; }

        public CreateAppointmentTemplate(IProcedure procedure, DateTime appointmentDate = default, DateTime appointmentTime = default)
        {
            Procedure = procedure;
            AppointmentDate = appointmentDate;
            AppointmentTime = appointmentTime;
        }
    }
}
