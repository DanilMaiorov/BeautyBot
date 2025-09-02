using BeautyBot.src.BeautyBot.Core.DataAcess.Context;
using BeautyBot.src.BeautyBot.Core.DataAcess.Mappers;
using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using LinqToDB;
using LinqToDB.Data;
using System.Reflection;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class PostgreSqlSlotRepository : ISlotRepository
    {
        //private readonly Dictionary<DateOnly, Dictionary<TimeOnly, Appointment>> _slots = new();

        private readonly IDataContextFactory<BeautyBotDataContext> _factory;
        public PostgreSqlSlotRepository(IDataContextFactory<BeautyBotDataContext> factory)
        {
            _factory = factory;
        }

        //public async Task<Dictionary<DateOnly, Dictionary<TimeOnly, Appointment>>> GetAllDaySlots()
        //{
        //    return await Task.FromResult(_slots);
        //}

        //public Task<Dictionary<TimeOnly, Appointment>> GetCurrentDayAvailableTimeSlots(DateOnly date, CancellationToken ct)
        //{
        //    if (!_slots.TryGetValue(date, out var slots))
        //    {   
        //        slots = GenerateTimeSlots();
        //        _slots[date] = slots;
        //    }

        //    var availableSlots = slots
        //        .Where(slot => slot.Value == null)
        //        .ToDictionary(pair => pair.Key, pair => pair.Value);

        //    return Task.FromResult(availableSlots);
        //}

        public async Task AddRange(IEnumerable<Slot> entities, CancellationToken ct)
        {
            var dbContext = _factory.CreateDataContext();

            var slotsModels = entities.Select(SlotModelMapper.MapToModel);

            await dbContext.Slots.BulkCopyAsync(slotsModels, ct);
        }

        public Task Add(Slot entity, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Slot>> GetSlotsByDate(DateOnly date)
        {
            var dbContext = _factory.CreateDataContext();

            var slots = await dbContext.Slots
                .Where(s => s.Date.Year == date.Year && s.Date.Month == date.Month && s.Date.Day == date.Day)
                .ToListAsync();

            return slots.Select(SlotModelMapper.MapFromModel);
        }

        public async Task<bool> AnySlotsExist()
        {
            var dbContext = _factory.CreateDataContext();

            var count = await dbContext.GetTable<SlotModel>().CountAsync();

            return count > 0;
        }



        public async Task UpdateSlot(DateOnly date, TimeOnly time, Guid appointmentId, CancellationToken ct)
        {
            var dbContext = _factory.CreateDataContext();

            var targetDateTime = date.ToDateTime(time);

            await dbContext.Slots
                .Where(s => s.Date.Year == date.Year && s.Date.Month == date.Month && s.Date.Day == date.Day && s.StartTime.Hour == time.Hour && s.StartTime.Minute == time.Minute)
                .Set(s => s.AppointmentId, appointmentId)
                .UpdateAsync();
        }


        //private IEnumerable<Slot> GenerateTimeSlots(DateTime date, int intervalMinutes)
        //{
        //    var startTime = new DateTime(date.Year, date.Month, date.Day, 10, 0, 0);
        //    var endTime = new DateTime(date.Year, date.Month, date.Day, 19, 0, 0);
        //    var interval = TimeSpan.FromMinutes(intervalMinutes);

        //    for (var time = startTime; time < endTime; time = time.Add(interval))
        //    {
        //        yield return new Slot{ 
        //            Date = date,
        //            StartTime = time,
        //            Duration = (int)interval.TotalMinutes,
        //            AppointmentId = null
        //        };
        //    }          
        //}


        //public async Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct)
        //{
        //    if (_slots.Count == 0)
        //        return await Task.FromResult(new List<DateOnly>());

        //    var days = _slots
        //        .Where(day => day.Value.All(item => item.Value != null))
        //        .Select(day => day.Key)
        //        .ToList();

        //    return await Task.FromResult(days);
        //}

        //public async Task<List<DateOnly>> GetAvailableDaySlots(CancellationToken ct)
        //{
        //    if (_slots.Count == 0)
        //        return await Task.FromResult(new List<DateOnly>());

        //    var days = _slots
        //        .Where(day => day.Value.Any(item => item.Value != null))
        //        .Select(day => day.Key)
        //        .ToList();

        //    return await Task.FromResult(days);
        //}


    }
}
