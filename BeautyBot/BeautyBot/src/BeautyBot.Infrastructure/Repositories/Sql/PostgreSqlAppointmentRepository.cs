using BeautyBot.src.BeautyBot.Core.DataAcess.Context;
using BeautyBot.src.BeautyBot.Core.DataAcess.Mappers;
using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Entities.Repositories;
using LinqToDB;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class PostgreSqlAppointmentRepository : IAppointmentRepository
    {

        private readonly IDataContextFactory<BeautyBotDataContext> _factory;

        public PostgreSqlAppointmentRepository(IDataContextFactory<BeautyBotDataContext> factory)
        {
            _factory = factory;
        }

        public async Task Add(Appointment appointment, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            await dbContext.InsertAsync(AppointmentModelMapper.MapToModel(appointment));
        }

        public async Task<IReadOnlyList<Appointment>> GetAllAppointmentsByUserId(Guid userId, CancellationToken ct)
        {
            if (userId == Guid.Empty)
                return new List<Appointment>().AsReadOnly();

            using var dbContext = _factory.CreateDataContext();

            var appointmentModels = await dbContext.Appointments
                .Where(x => x.User.UserId == userId)
                .LoadWith(i => i.User)
                .LoadWith(i => i.Procedure)
                .ToListAsync(ct);

            var appointmentEntities = appointmentModels.Select(AppointmentModelMapper.MapFromModel).ToList();

            return appointmentEntities.ToList().AsReadOnly();
        }        

        public async Task<IReadOnlyList<Appointment>> GetActiveAppointmentsByUserId(Guid userId, CancellationToken ct)
        {
            if (userId == Guid.Empty)
                return new List<Appointment>().AsReadOnly();
            
            using var dbContext = _factory.CreateDataContext();

            var appointmentModels = await dbContext.Appointments
                .Where(x => x.User.UserId == userId && x.State == AppointmentState.Active)
                .LoadWith(i => i.User)
                .LoadWith(i => i.Procedure)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync(ct);

            var appointmentEntities = appointmentModels.Select(AppointmentModelMapper.MapFromModel).ToList();

            return appointmentEntities.ToList().AsReadOnly();
        }

        public async Task<Appointment?> GetAppointment(Guid appointmentId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var appointmentModel = await dbContext.Appointments
                .Where(x => x.Id == appointmentId)
                .LoadWith(i => i.User)
                .LoadWith(i => i.Procedure)
                .FirstOrDefaultAsync(ct);

            return AppointmentModelMapper.MapFromModel(appointmentModel);
        }

        public async Task UpdateAppointment(Appointment appointment, CancellationToken ct)
        {
            //using var dbContext = _factory.CreateDataContext();

            //var updateIndex = _appointments.FindIndex(x => x.Id == appointment.Id);

            //if (updateIndex != -1)
            //{
            //    _appointments[updateIndex] = appointment;

            //    //сделаю искусственную задержку для асинхронности
            //    await Task.Delay(1, ct);
            //}
            //else
            //{
            //    throw new KeyNotFoundException($"Запись с номером {appointment.Id} не найдена");
            //}
        }

        //тут продолжить
        public async Task CancelAppointment(Guid appointmentId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var appointmentEntity = await GetAppointment(appointmentId, ct);

            appointmentEntity.Id = default;

            await dbContext.Appointments
                .Where(x => x.Id == appointmentId)
                .DeleteAsync(ct);

            await dbContext.Slots
                .Where(x => x.AppointmentId == appointmentId)
                .Set(i => i.AppointmentId, () => null)
                .UpdateAsync(ct);
        }
    }
}
