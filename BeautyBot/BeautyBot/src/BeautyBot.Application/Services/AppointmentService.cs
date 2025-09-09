using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Entities.Repositories;
using BeautyBot.src.BeautyBot.Domain.Services;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<Appointment> GetAppointment(Guid appointmentId, CancellationToken ct)
        {
            return await _appointmentRepository.GetAppointment(appointmentId, ct);
        }

        // реализация метода интерфейса Add
        public async Task<Appointment> AddAppointment(BeautyBotUser user, IProcedure procedure, DateOnly date,TimeOnly time, CancellationToken ct)
        {
            var newAppointment = new Appointment(user, procedure, date, time);

            await _appointmentRepository.Add(newAppointment, ct);

            return newAppointment;
        }

        public async Task<IReadOnlyList<Appointment>> GetUserAppointmentsByUserId(Guid userId, CancellationToken ct)
        {
            return await _appointmentRepository.GetAllAppointmentsByUserId(userId, ct);
        }

        public async Task<IReadOnlyList<Appointment>> GetUserActiveAppointmentsByUserId(Guid userId, CancellationToken ct)
        {
            return await _appointmentRepository.GetActiveAppointmentsByUserId(userId, ct);
        }

        public async Task UpdateAppointment(Guid appointmentId, AppointmentState state, CancellationToken ct)
        {
            var udpateAppointment = await _appointmentRepository.GetAppointment(appointmentId, ct);

            udpateAppointment.State = state;
            udpateAppointment.StateChangedAt = DateTime.Now;

            await _appointmentRepository.UpdateAppointment(udpateAppointment, ct);
        }

        public async Task EditAppointment(Guid appointmentId, DateTime date, CancellationToken ct)
        {
            var editAppointment = await _appointmentRepository.GetAppointment(appointmentId, ct);

            editAppointment.AppointmentDate = date;

            await _appointmentRepository.UpdateAppointment(editAppointment, ct);
        }

        public async Task CancelAppointment(Guid appointmentId, CancellationToken ct)
        {
            await _appointmentRepository.CancelAppointment(appointmentId, ct);
        }
    }
}
