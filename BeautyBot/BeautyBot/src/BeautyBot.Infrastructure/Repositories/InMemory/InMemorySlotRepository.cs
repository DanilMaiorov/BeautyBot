using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using System;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class InMemorySlotRepository : ISlotRepository
    {
        private readonly Dictionary<DateOnly, Dictionary<TimeOnly, Appointment>> _slots = new();

        public async Task<Dictionary<DateOnly, Dictionary<TimeOnly, Appointment>>> GetAllDaySlots()
        {
            return await Task.FromResult(_slots);
        }

        public Task<Dictionary<TimeOnly, Appointment>> GetCurrentDayAvailableTimeSlots(DateOnly date, CancellationToken ct)
        {
            if (!_slots.TryGetValue(date, out var slots))
            {   
                slots = GenerateTimeSlots();
                _slots[date] = slots;
            }

            var availableSlots = slots
                .Where(slot => slot.Value == null)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return Task.FromResult(availableSlots);
        }


        public Task Add(Appointment appointment, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateSlot(Appointment appointment, CancellationToken ct)
        {
            var date = DateOnly.FromDateTime(appointment.AppointmentDate);
            var time = TimeOnly.FromDateTime(appointment.AppointmentDate);

            var currentDaySlots = _slots[date];

            await Task.FromResult(currentDaySlots[time] = appointment);
        }



        private Dictionary<TimeOnly, Appointment> GenerateTimeSlots()
        {
            Dictionary<TimeOnly, Appointment> slots = new();

            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(10, 0);
            var interval = TimeSpan.FromMinutes(60);

            for (var time = startTime; time <= endTime; time = time.Add(interval))
                slots[time] = null;
            
            return slots;
        }



        public async Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct)
        {
            if (_slots.Count == 0)
                return await Task.FromResult(new List<DateOnly>());

            var days = _slots
                .Where(day => day.Value.All(item => item.Value != null))
                .Select(day => day.Key)
                .ToList();

            return await Task.FromResult(days);
        }

        public async Task<List<DateOnly>> GetAvailableDaySlots(CancellationToken ct)
        {
            if (_slots.Count == 0)
                return await Task.FromResult(new List<DateOnly>());

            var days = _slots
                .Where(day => day.Value.Any(item => item.Value != null))
                .Select(day => day.Key)
                .ToList();

            return await Task.FromResult(days);
        }
    }
}
