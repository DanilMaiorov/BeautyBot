using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    // Репозиторий для записей (назначений)
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IReadOnlyList<Appointment>> GetAllAppointmentsByUserId(Guid userId, CancellationToken ct);
        Task<IReadOnlyList<Appointment>> GetActiveAppointmentsByUserId(Guid userId, CancellationToken ct);
        Task<Appointment?> GetAppointment(Guid appointmentId, CancellationToken ct);
        Task UpdateAppointment(Appointment appointment, CancellationToken ct);
    }
}
