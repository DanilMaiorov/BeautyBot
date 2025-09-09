using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Entities.Repositories;
using BeautyBot.src.BeautyBot.Domain.Services;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class SlotService : ISlotService
    {
        private readonly ISlotRepository _slotRepository;

        CancellationToken ct;
        public SlotService(ISlotRepository repository)
        {
            _slotRepository = repository;
        }

        public IEnumerable<Slot> GenerateDailySlots(DateTime date, int intervalMinutes)
        {
            var startTime = new DateTime(date.Year, date.Month, date.Day, 10, 0, 0);
            var endTime = new DateTime(date.Year, date.Month, date.Day, 19, 0, 0);
            var interval = TimeSpan.FromMinutes(intervalMinutes);

            for (var time = startTime; time < endTime; time = time.Add(interval))
            {
                yield return new Slot
                {
                    Date = date,
                    StartTime = time,
                    Duration = (int)interval.TotalMinutes,
                    AppointmentId = null
                };
            }
        }

        public async Task GenerateYearlySlots(CancellationToken ct)
        {
            if (await _slotRepository.AnySlotsExist())
                return;

            var allSlots = new List<Slot>();
            var currentDate = DateTime.Today;
            var lastDay = currentDate.AddYears(1);

            for (var date = currentDate; date < lastDay; date = date.AddDays(1))
            {
                var dailySlots = GenerateDailySlots(date, 90);

                allSlots.AddRange(dailySlots);
            }
            await _slotRepository.AddRange(allSlots, ct);
        }

        public async Task<IEnumerable<Slot>> GetSlotsByDate(DateOnly date, CancellationToken ct)
        {
            var slots = await _slotRepository.GetSlotsByDate(date);

            return slots.Where(slot => slot.AppointmentId == null);
        }

        public async Task<List<DateOnly>> GetUnavailableSlotsByDate(CancellationToken ct)
        {
            return await _slotRepository.GetUnavailableDaySlots(ct);
        }

        public async Task UpdateSlotFromAppointment(Appointment appointment, CancellationToken ct)
        {
            await _slotRepository.UpdateSlot(
                DateOnly.FromDateTime(appointment.AppointmentDate),
                TimeOnly.FromDateTime(appointment.AppointmentDate),
                appointment.Id,
                ct
            );
        }

        public async Task ResetSlotFromAppointment(Appointment appointment, CancellationToken ct)
        {
            await _slotRepository.ResetSlot(
                DateOnly.FromDateTime(appointment.AppointmentDate),
                TimeOnly.FromDateTime(appointment.AppointmentDate),
                appointment.Id,
                ct
            );
        }
    }   
}
