using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class InMemorySlotRepository : ISlotRepository
    {
        private readonly Dictionary<DateOnly, Dictionary<TimeOnly, bool>> _slots = new();

        public Task<Dictionary<TimeOnly, bool>> GetSlots(DateOnly date, CancellationToken ct)
        {

            if (_slots.TryGetValue(date, out var slots))
            {
                slots.Where(slot => !slot.Value);
            }
            else
            {
                slots = SlotsGenerator();

                _slots[date] = slots;
            }



            return Task.FromResult(slots);
        }
        public Task Add(Appointment appointment, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        //private bool IsBooked()
        //{
        //    return _days.Any();
        //}

        private Dictionary<TimeOnly, bool> SlotsGenerator()
        {
            Dictionary<TimeOnly, bool> s = new();

            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(18, 0);
            var interval = TimeSpan.FromMinutes(90);

            for (var time = startTime; time <= endTime; time = time.Add(interval))
            {
                s[time] = false;
            }

            return s;
        }
    }
}
