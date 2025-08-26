using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class InMemoryAppointmentRepository : IAppointmentRepository
    {
        private readonly List<Appointment> _appointments = new List<Appointment>();
        //public IReadOnlyList<Appointment> GetActiveByUserId(Guid userId) => _appointments.Where(a => a.UserId == userId && a.Status == AppointmentStatus.Active).ToList().AsReadOnly();
        //public Appointment? GetById(Guid id) => _appointments.FirstOrDefault(a => a.Id == id);
        //public IReadOnlyList<Appointment> GetAll() => _appointments.AsReadOnly();
        //public void Update(Appointment entity) { /* ... */ }
        //public void Delete(Guid id) { _appointments.RemoveAll(a => a.Id == id); }

        public async Task Add(Appointment entity, CancellationToken ct)
        {
            _appointments.Add(entity);
            await Task.Delay(1);
        }

        public async Task<IReadOnlyList<Appointment>> GetAllAppointmentsByUserId(Guid userId, CancellationToken ct)
        {
            var result = _appointments
                .Where(x => x.User.UserId == userId)
                .ToList()
                .AsReadOnly();

            //сделаю искусственную задержку для асинхронности
            await Task.Delay(1, ct);

            return result;
        }        

        public async Task<IReadOnlyList<Appointment>> GetActiveAppointmentsByUserId(Guid userId, CancellationToken ct)
        {
            if (userId == Guid.Empty)
            {
                return _appointments.AsReadOnly();
            }

            var result = _appointments
                .Where(x => x.User.UserId == userId && x.State == AppointmentState.Active)
                .ToList()
                .AsReadOnly();

            //сделаю искусственную задержку для асинхронности
            await Task.Delay(1);

            return result;
        }

        public async Task<Appointment?> GetAppointment(Guid appointmentId, CancellationToken ct)
        {
            var item = _appointments.FirstOrDefault(x => x.Id == appointmentId);

            //сделаю искусственную задержку для асинхронности
            await Task.Delay(1, ct);

            return item;
        }

        public async Task UpdateAppointment(Appointment appointment, CancellationToken ct)
        {
            var updateIndex = _appointments.FindIndex(x => x.Id == appointment.Id);

            if (updateIndex != -1)
            {
                _appointments[updateIndex] = appointment;

                //сделаю искусственную задержку для асинхронности
                await Task.Delay(1, ct);
            }
            else
            {
                throw new KeyNotFoundException($"Запись с номером {appointment.Id} не найдена");
            }
        }

        public async Task CancelAppointment(Guid appointmentId, CancellationToken ct)
        {
            _appointments.RemoveAll(x => x.Id == appointmentId);

            //сделаю искусственную задержку для асинхронности
            await Task.Delay(1, ct);
        }
    }
}
