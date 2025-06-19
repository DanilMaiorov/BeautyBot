using BeautyBot.src.BeautyBot.Domain.Repositories;
using BeautyBot.src.BeautyBot.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IProcedureDefinitionRepository _procedureDefinitionRepository; // Для получения деталей процедуры

        public AppointmentService(IAppointmentRepository appointmentRepository, IProcedureDefinitionRepository procedureDefinitionRepository)
        {
            _appointmentRepository = appointmentRepository;
            _procedureDefinitionRepository = procedureDefinitionRepository;
        }

        //public IReadOnlyList<Appointment> GetUserAppointments(Guid userId) => _appointmentRepository.GetAllByUserId(userId);
        //public IReadOnlyList<Appointment> GetUserActiveAppointments(Guid userId) => _appointmentRepository.GetActiveByUserId(userId);

        //public Appointment BookAppointment(Guid userId, Guid procedureId, DateTime appointmentTime)
        //{
        //    var procedure = _procedureDefinitionRepository.GetById(procedureId);
        //    if (procedure == null) throw new ArgumentException("Procedure not found.");

        //    var appointment = new Appointment(userId, procedure, appointmentTime);
        //    _appointmentRepository.Add(appointment);
        //    return appointment;
        //}

        //public void CancelAppointment(Guid appointmentId)
        //{
        //    var appointment = _appointmentRepository.GetById(appointmentId);
        //    if (appointment != null)
        //    {
        //        appointment.Status = AppointmentStatus.Cancelled;
        //        _appointmentRepository.Update(appointment);
        //    }
        //}

        //public void CompleteAppointment(Guid appointmentId)
        //{
        //    var appointment = _appointmentRepository.GetById(appointmentId);
        //    if (appointment != null)
        //    {
        //        appointment.Status = AppointmentStatus.Completed;
        //        _appointmentRepository.Update(appointment);
        //    }
        //}
    }
}
