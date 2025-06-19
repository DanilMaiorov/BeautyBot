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
        //private readonly List<Appointment> _appointments = new List<Appointment>();
        //public IReadOnlyList<Appointment> GetAllByUserId(Guid userId) => _appointments.Where(a => a.UserId == userId).ToList().AsReadOnly();
        //public IReadOnlyList<Appointment> GetActiveByUserId(Guid userId) => _appointments.Where(a => a.UserId == userId && a.Status == AppointmentStatus.Active).ToList().AsReadOnly();
        //public Appointment? GetById(Guid id) => _appointments.FirstOrDefault(a => a.Id == id);
        //public IReadOnlyList<Appointment> GetAll() => _appointments.AsReadOnly();
        //public void Add(Appointment entity) { _appointments.Add(entity); }
        //public void Update(Appointment entity) { /* ... */ }
        //public void Delete(Guid id) { _appointments.RemoveAll(a => a.Id == id); }
    }
}
