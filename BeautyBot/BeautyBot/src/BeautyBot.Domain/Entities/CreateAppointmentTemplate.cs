using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class CreateAppointmentTemplate
    {
        public IProcedure Procedure { get; set; } // храню интерфейс или конкретный класс
        public string AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }

        public CreateAppointmentTemplate(IProcedure procedure, string appointmentDate = null, string appointmentTime = null)
        {
            Procedure = procedure;
            AppointmentDate = appointmentDate;
            AppointmentTime = appointmentTime;
        }
    }
}
