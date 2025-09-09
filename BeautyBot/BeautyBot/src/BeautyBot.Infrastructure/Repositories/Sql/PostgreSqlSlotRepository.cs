using BeautyBot.src.BeautyBot.Core.DataAcess.Context;
using BeautyBot.src.BeautyBot.Core.DataAcess.Mappers;
using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Entities.Repositories;
using LinqToDB;
using LinqToDB.Data;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class PostgreSqlSlotRepository : ISlotRepository
    {
        private readonly IDataContextFactory<BeautyBotDataContext> _factory;
        public PostgreSqlSlotRepository(IDataContextFactory<BeautyBotDataContext> factory)
        {
            _factory = factory;
        }
        public async Task AddRange(IEnumerable<Slot> entities, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var slotsModels = entities.Select(SlotModelMapper.MapToModel);

            await dbContext.Slots.BulkCopyAsync(slotsModels, ct);
        }

        public Task Add(Slot entity, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Slot>> GetSlotsByDate(DateOnly date)
        {
            using var dbContext = _factory.CreateDataContext();

            var slots = await dbContext.Slots
                .Where(s => s.Date.Year == date.Year && s.Date.Month == date.Month && s.Date.Day == date.Day)
                .ToListAsync();

            return slots.Select(SlotModelMapper.MapFromModel);
        }

        public async Task<bool> AnySlotsExist()
        {
            using var dbContext = _factory.CreateDataContext();

            var count = await dbContext.GetTable<SlotModel>().CountAsync();

            return count > 0;
        }
        public async Task UpdateSlot(DateOnly date, TimeOnly time, Guid appointmentId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var targetDateTime = date.ToDateTime(time);

            await dbContext.Slots
                .Where(s => s.StartTime == targetDateTime)
                .Set(s => s.AppointmentId, appointmentId)
                .UpdateAsync();
        }
        public async Task ResetSlot(DateOnly date, TimeOnly time, Guid appointmentId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var targetDateTime = date.ToDateTime(time);

            await dbContext.Slots
                .Where(s => s.StartTime == targetDateTime)
                .Set(s => s.AppointmentId, (Guid?)null)
                .UpdateAsync(ct);
        }

        public async Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var unavailableDays = await dbContext.Slots
                .GroupBy(s => s.StartTime.Date)
                .Where(g => g.Count() == g.Count(s => s.AppointmentId != null))
                .Select(g => DateOnly.FromDateTime(g.Key))
                .ToListAsync(ct);

            return unavailableDays;
        }
    }
}
