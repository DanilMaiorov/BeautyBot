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
        private readonly IProcedureDefinitionRepository _procedureDefinitionRepository; // Для получения деталей процедуры

        public AppointmentService(IAppointmentRepository appointmentRepository, IProcedureDefinitionRepository procedureDefinitionRepository)
        {
            _appointmentRepository = appointmentRepository;
            _procedureDefinitionRepository = procedureDefinitionRepository;
        }

        // реализация метода интерфейса Add
        public async Task<Appointment> AddAppointment(BeautyBotUser user, IProcedure procedure, DateTime appointmentTime, CancellationToken ct)
        {
            //var tasks = await GetAllByUserId(user.UserId, ct);
            //if (tasks.Count >= maxTaskAmount)
            //    throw new TaskCountLimitException(maxTaskAmount);
            //string newTask = Validate.ValidateString(name, maxTaskLength);
            //написать метод проверки дубликатов добавленных записей
            //Helper.CheckDuplicate(newTask, tasks);

            var newAppointment = new Appointment(user.UserId, procedure, DateTime.Now);

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

        public async Task CancelAppointment(Guid appointmentId, CancellationToken ct)
        {
            await _appointmentRepository.CancelAppointment(appointmentId, ct);
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
