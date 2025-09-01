using BeautyBot.src.BeautyBot.Domain.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface ISlotService
    {
        //Task<Dictionary<TimeOnly, bool>> GetSlots(DateOnly date, CancellationToken ct);

        //Task UpdateSlot(Appointment appointment, CancellationToken ct);

        //Task<Dictionary<TimeOnly, Appointment>> GetCurrentDayAvailableTimeSlots(DateOnly date, CancellationToken ct);

        //Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct);

        //Task<List<DateOnly>> GetAvailableDaySlots(CancellationToken ct);

        IEnumerable<Slot> GenerateDailySlots(DateTime date, int intervalMinutes);
        Task GenerateYearlySlots(CancellationToken ct);
    }
}
