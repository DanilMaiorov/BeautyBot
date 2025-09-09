using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src.BeautyBot.Domain.Entities.Repositories
{
    // Репозиторий для записей (назначений)
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task Add(Appointment appointment, CancellationToken ct);
        Task UpdateAppointment(Appointment appointment, CancellationToken ct);
        Task<IReadOnlyList<Appointment>> GetAllAppointmentsByUserId(Guid userId, CancellationToken ct);
        Task<IReadOnlyList<Appointment>> GetActiveAppointmentsByUserId(Guid userId, CancellationToken ct);
        Task<Appointment?> GetAppointment(Guid appointmentId, CancellationToken ct);
        Task CancelAppointment(Guid appointmentId, CancellationToken ct);
    }
}
