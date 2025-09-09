using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface IAppointmentService
    {
        Task<IReadOnlyList<Appointment>> GetUserAppointmentsByUserId(Guid userId, CancellationToken ct);
        Task<IReadOnlyList<Appointment>> GetUserActiveAppointmentsByUserId(Guid userId, CancellationToken ct);
        Task<Appointment> AddAppointment(BeautyBotUser user, IProcedure procedure, DateOnly date, TimeOnly time, CancellationToken ct);
        Task CancelAppointment(Guid appointmentId, CancellationToken ct);
        Task UpdateAppointment(Guid appointmentId, AppointmentState state, CancellationToken ct);
        Task EditAppointment(Guid appointmentId, DateTime date, CancellationToken ct);
        Task<Appointment> GetAppointment(Guid appointmentId, CancellationToken ct);
    }
}
