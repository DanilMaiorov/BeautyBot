using BeautyBot.src.BeautyBot.Core.DataAcess.Context;
using BeautyBot.src.BeautyBot.Core.DataAcess.Mappers;
using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using LinqToDB;
using System.Security.AccessControl;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class PostgreSqlAppointmentRepository : IAppointmentRepository
    {

        private readonly IDataContextFactory<BeautyBotDataContext> _factory;

        public PostgreSqlAppointmentRepository(IDataContextFactory<BeautyBotDataContext> factory)
        {
            _factory = factory;
        }


        private readonly List<Appointment> _appointments = new List<Appointment>();


        public async Task Add(Appointment appointment, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            await dbContext.InsertAsync(AppointmentModelMapper.MapToModel(appointment));
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
