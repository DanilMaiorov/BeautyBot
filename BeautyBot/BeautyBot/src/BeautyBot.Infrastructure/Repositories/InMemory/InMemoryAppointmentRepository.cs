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
                .Where(x => x.UserId == userId)
                .ToList()
                .AsReadOnly();

            //сделаю искусственную задержку для асинхронности
            await Task.Delay(1, ct);

            return result;
        }        


        public async Task<IReadOnlyList<Appointment>> GetActiveAppointmentsByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return _appointments.AsReadOnly();
            }

            var result = _appointments
                .Where(x => x.UserId == userId && x.Status == AppointmentStatus.Active)
                .ToList()
                .AsReadOnly();

            //сделаю искусственную задержку для асинхронности
            await Task.Delay(1);

            return result;
        }
    }
}
