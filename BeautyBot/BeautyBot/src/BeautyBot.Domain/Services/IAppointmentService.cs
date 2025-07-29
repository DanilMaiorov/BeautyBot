using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    // Новый сервис для работы с записями (бронированием)
    public interface IAppointmentService
    {
        Task<IReadOnlyList<Appointment>> GetUserAppointmentsByUserId(Guid userId, CancellationToken ct);
        Task<IReadOnlyList<Appointment>> GetUserActiveAppointmentsByUserId(Guid userId, CancellationToken ct);
        Task<Appointment> AddAppointment(BeautyBotUser user, IProcedure procedure, DateTime date, CancellationToken ct);

        Task CancelAppointment(Guid appointmentId, CancellationToken ct);

        //void CompleteAppointment(Guid appointmentId);

        Task UpdateAppointment(Guid appointmentId, AppointmentState state, CancellationToken ct);
    }
}
