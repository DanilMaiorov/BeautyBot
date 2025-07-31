using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class InMemorySlotRepository : ISlotRepository
    {
        private readonly Dictionary<DateOnly, Dictionary<TimeOnly, bool>> _slots = new();

        public async Task<Dictionary<DateOnly, Dictionary<TimeOnly, bool>>> GetAllDaySlots()
        {
            return await Task.FromResult(_slots);
        }

        public Task<Dictionary<TimeOnly, bool>> GetCurrentDayAvailableTimeSlots(DateOnly date, CancellationToken ct)
        {
            if (!_slots.TryGetValue(date, out var slots))
            {   
                slots = GenerateTimeSlots();
                _slots[date] = slots;
            }

            var availableSlots = slots
                .Where(slot => !slot.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return Task.FromResult(availableSlots);
        }


        public Task Add(Appointment appointment, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateSlot(DateOnly date, TimeOnly time)
        {
            var currentDaySlots = _slots[date];

            await Task.FromResult(currentDaySlots[time] = true);
        }



        private Dictionary<TimeOnly, bool> GenerateTimeSlots()
        {
            Dictionary<TimeOnly, bool> slots = new();

            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(10, 0);
            var interval = TimeSpan.FromMinutes(60);

            for (var time = startTime; time <= endTime; time = time.Add(interval))
                slots[time] = false;
            
            return slots;
        }



        public async Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct)
        {
            if (_slots.Count == 0)
                return await Task.FromResult(new List<DateOnly>());

            var days = _slots
                .Where(day => day.Value.All(item => item.Value))
                .Select(day => day.Key)
                .ToList();

            return await Task.FromResult(days);
        }

        public async Task<List<DateOnly>> GetAvailableDaySlots(CancellationToken ct)
        {
            if (_slots.Count == 0)
                return await Task.FromResult(new List<DateOnly>());

            var days = _slots
                .Where(day => day.Value.Any(item => !item.Value))
                .Select(day => day.Key)
                .ToList();

            return await Task.FromResult(days);
        }
    }
}
